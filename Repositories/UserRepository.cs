using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using portal_agile.Data;
using portal_agile.Exceptions.Users;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Repositories
{
    public class UserRepository : BaseRepository<User, string, AppDbContext>, IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(
            AppDbContext context,
            UserManager<User> userManager)
            : base(context)
        {
            _userManager = userManager;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            return user;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByUserName(string userName)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            return user;
        }

        public async Task AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId)
                       ?? throw new UserNotFoundException($"User with ID {userId} not found.");

            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains(roleName))
            {
                throw new InvalidOperationException($"User already has the role {roleName}");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role: {errors}");
            }
        }

        public async Task<bool> AssignPermissionToUser(string userId, int permissionId, string modifiedBy)
        {
            var user = await _userManager.FindByIdAsync(userId)
                       ?? throw new UserNotFoundException($"User with ID {userId} not found.");

            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {permissionId} not found.");
            }

            // Check if permission already exists
            bool exists = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            if (exists)
                return true;

            _context.UserPermissions.Add(new UserPermission
            {
                UserId = user.Id,
                PermissionId = permission.PermissionId,
                CreatedBy = modifiedBy,
                CreatedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                       ?? throw new UserNotFoundException($"User with ID {userId} not found.");

            user.IsActive = false;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lock the user for 100 years

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to deactivate user: {errors}");
            }

            return user;
        }
    }
}
