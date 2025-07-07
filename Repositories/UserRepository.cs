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

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            var userByToken = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken &&
                                        u.RefreshTokenExpiry > DateTime.UtcNow);

            return userByToken;
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
