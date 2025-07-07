using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using portal_agile.Data;
using portal_agile.Dtos.Modules;
using portal_agile.Dtos.Permissions;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;
using System.Linq;
using System.Security.Claims;

namespace portal_agile.Repositories
{
    public class PermissionRepository : BaseRepository<Permission, int, AppDbContext>, IPermissionRepository
    {
        private readonly UserManager<User> _userManager;

        private readonly IRoleRepository _roleRepository;

        public PermissionRepository(
            AppDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IRoleRepository roleRepository)
            : base(context)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>> GetPermissionsByRoleId(string roleId)
        {
            var role = await _roleRepository.GetById(roleId) ??
                throw new ArgumentException($"Role with ID {roleId} not found.");

            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return permissions;
        }

        /// <inheritdoc/>
        public async Task<List<Permission>> GetPermissionsByRolesAsync(IList<string> roles)
        {
            return await _context.Permissions
                .Where(p => p.IsActive && !p.IsDeleted)
                .Where(p => _context.RolePermissions
                    .Where(rp => roles.Contains(rp.Role.Name!))
                    .Select(rp => rp.PermissionId)
                    .Contains(p.PermissionId))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Permission>> GetAllUserPermissions(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            // Get user's roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // Get all permissions from all user's roles
            var rolePermissions = new List<Permission>();
            foreach (var roleName in userRoles)
            {
                var role = await _roleRepository.GetRoleByName(roleName);
                if (role != null)
                {
                    var permissions = await _roleRepository.GetRolePermissions(role.Id);
                    rolePermissions.AddRange(permissions ?? []);
                }
            }

            return rolePermissions;
        }

        /// <inheritdoc/>
        public async Task<IList<Claim>> GetUserPermissionClaims(string userId)
        {
            var permissions = await GetAllUserPermissions(userId);
            return permissions
                .Select(p => new Claim("Permission", p.Code))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> AssignPermissionsToRole(string roleId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            var role = await _roleRepository.GetById(roleId);
            if (role == null)
                return false;

            // Check if the role is a system role
            if (role.IsSystemRole && (role.Name == "Admin" || role.Name == "SuperAdmin"))
            {
                // For SuperAdmin, we ensure it always has ALL permissions
                if (role.Name == "SuperAdmin")
                {
                    var allPermissions = await GetAll();
                    permissionIds = allPermissions.Select(p => p.PermissionId);
                }
            }

            // Begin transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Remove existing role permissions
                    var existingPermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == roleId)
                        .ToListAsync();

                    _context.RolePermissions.RemoveRange(existingPermissions);

                    // Add new role permissions
                    foreach (var permissionId in permissionIds)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId,
                            CreatedBy = modifiedBy
                        });
                    }

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

        /// <inheritdoc/>
        public async Task<bool> HasPermission(string userId, string permissionCode)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // SuperAdmin automatically has all permissions
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("SuperAdmin"))
                return true;

            // Check user's permissions
            var userPermissions = await GetAllUserPermissions(userId);
            return userPermissions.Any(p => p.Code == permissionCode);
        }

        private string GetModuleDisplayName(string moduleName)
        {
            return moduleName switch
            {
                "UserManagement" => "User Management",
                "RoleManagement" => "Role Management",
                "EmployeeManagement" => "Employee Management",
                "DepartmentManagement" => "Department Management",
                "PermissionManagement" => "Permission Management",
                "SystemAdministration" => "System Administration",
                _ => moduleName
            };
        }

        private string GetModuleRoute(string moduleName)
        {
            return moduleName switch
            {
                "UserManagement" => "/users",
                "RoleManagement" => "/roles",
                "EmployeeManagement" => "/employees",
                "DepartmentManagement" => "/departments",
                "PermissionManagement" => "/permissions",
                "SystemAdministration" => "/system",
                _ => $"/{moduleName.ToLower()}"
            };
        }
    }
}
