using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using portal_agile.Data;
using portal_agile.Dtos.Permissions;
using portal_agile.Exceptions.Users;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Repositories
{
    public class UserRepository : BaseRepository<User, string, AppDbContext>, IUserRepository
    {
        private readonly UserManager<User> _userManager;

        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public UserRepository(
            AppDbContext context,
            UserManager<User> userManager,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository)
            : base(context)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByUserName(string userName)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<IEnumerable<Role>?> GetRolesByUserId(string userId)
        {
            return await _userManager.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles)
                .Select(ur => ur.Role)
                .ToListAsync();
        }
        //var removed = await _userManager.RemoveFromRolesAsync(user, roles);

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>?> GetDirectPermissionsByUserId(string userId)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                .Include(u => u.DirectPermissions)
                    .ThenInclude(dp => dp.Permission)
                .FirstOrDefaultAsync() ?? throw new UserNotFoundException(userId);

            var rolePermissionIds = user.UserRoles?
                .Where(ur => ur.Role != null)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.PermissionId)
                .ToHashSet() ?? [];

            if (rolePermissionIds == null || rolePermissionIds.Count == 0)
                return [];

            var directPermissionsNotInRoles = user.DirectPermissions?
                .Where(dp => !rolePermissionIds.Contains(dp.PermissionId))
                .Select(dp => dp.Permission)
                .ToList() ?? [];

            return directPermissionsNotInRoles;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>?> GetAllUserPermissionsByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Get user's roles
            var userRoles = await _userManager.GetRolesAsync(user!);

            // Get all permissions from all user's roles
            var rolePermissions = new List<Permission>();
            foreach (var userRole in userRoles)
            {
                var role = await _roleRepository.GetRoleByName(userRole);
                if (role != null)
                {
                    var permissions = await _permissionRepository.GetPermissionsByRoleId(role.Id);
                    if (permissions?.Count() > 0)
                        rolePermissions.AddRange(permissions);
                }
            }

            // Get user's direct permissions
            var directPermissions = await GetDirectPermissionsByUserId(userId);

            if (directPermissions?.Count() > 0)
                _ = rolePermissions
                    .Union(directPermissions, new PermissionComparer())
                    .ToList();

            return rolePermissions;
        }

        /// <inheritdoc/>
        public async Task<IList<Claim>> GetUserPermissionClaimsByUserId(string userId)
        {
            var permissions = await GetAllUserPermissionsByUserId(userId);

            var claims = new List<Claim>();

            if (permissions?.Count() > 0)
                claims = permissions
                        .Select(p => new Claim("Permission", p.Code))
                        .ToList();

            return claims;
        }

        public async Task<bool> AssignRoleToUserByRoleName(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Check if the user already has the role
            if (await _userManager.IsInRoleAsync(user!, roleName))
                return true;

            // Assign the role to the user
            var result = await _userManager.AddToRoleAsync(user!, roleName);
            return result.Succeeded;
        }

        /// <inheritdoc/>
        public async Task<bool> AssignUserDirectPermissions(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Begin transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Remove existing user permissions
                    var existingPermissions = await _context.UserPermissions
                        .Where(up => up.UserId == userId)
                        .ToListAsync();

                    _context.UserPermissions.RemoveRange(existingPermissions);

                    // Add new user permissions
                    var newPermissions = permissionIds.Select(pid => new UserPermission
                    {
                        UserId = userId,
                        PermissionId = pid,
                        CreatedBy = modifiedBy
                    });

                    _context.UserPermissions.AddRange(newPermissions);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> AssignPermissionToUser(string userId, int permissionId, string modifiedBy)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var permission = await _context.Permissions.FindAsync(permissionId);

            // Check if permission already exists
            bool exists = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            if (exists)
                return true;

            _context.UserPermissions.Add(new UserPermission
            {
                UserId = user!.Id,
                PermissionId = permission!.PermissionId,
                CreatedBy = modifiedBy,
                CreatedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> RevokePermissionsFromUser(string userId, IEnumerable<int> permissionIds)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == user!.Id && permissionIds.Contains(up.PermissionId))
                .ToListAsync();

            if (!userPermissions.Any())
                return true; // Nothing to remove

            _context.UserPermissions.RemoveRange(userPermissions);

            var affectedRows = await _context.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> RevokeDirectPermissionsFromUser(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == user!.Id && permissionIds.Contains(up.PermissionId))
                .ToListAsync();

            if (userPermissions.Count == 0)
                return true;

            _context.UserPermissions.RemoveRange(userPermissions);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateUserByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user!.IsActive = false;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lock the user for 100 years

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
