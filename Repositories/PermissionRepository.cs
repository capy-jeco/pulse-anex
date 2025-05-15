using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using portal_agile.Data;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Repositories
{
    public class PermissionRepository : BaseRepository<Permission, AppDbContext>, IPermissionRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public PermissionRepository(
            AppDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
            : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>> GetPermissionsByRoleId(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                throw new ArgumentException($"Role with ID {roleId} not found.");
            }
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return permissions;
        }

        public async Task<Permission> GetPermissionById(int permissionId)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
            if (permission == null)
            {
                throw new ArgumentException($"Permission with ID {permissionId} not found.");
            }
            return permission;
        }

        public async Task<Dictionary<string, List<Permission>>> GetPermissionsByModule()
        {
            var permissions = await GetAllPermissions();
            return permissions.GroupBy(p => p.Module).ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<List<Permission>> GetRolePermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                throw new ArgumentException($"Role with ID {roleId} not found.");
            }

            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .Where(p => p.IsActive)
                .ToListAsync();

            return permissions;
        }

        public async Task<List<Permission>> GetUserDirectPermissions(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }
            var permissions = await _context.UserPermissions
                .Where(up => up.UserId == user.Id)
                .Include(up => up.Permission)
                .Select(up => up.Permission)
                .ToListAsync();

            return permissions;
        }

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
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var permissions = await GetRolePermissions(role.Id);
                    rolePermissions.AddRange(permissions);
                }
            }

            // Get user's direct permissions
            var directPermissions = await GetUserDirectPermissions(userId);

            return rolePermissions
                .Union(directPermissions, new PermissionComparer())
                .ToList();
        }

        public async Task<Permission> Store(Permission permission)
        {
            return await base.Create(permission);
        }


        public async Task<bool> AssignPermissionsToRole(string roleId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            // Check if the role is a system role
            if (role.IsSystemRole && (role.Name == "Admin" || role.Name == "SuperAdmin"))
            {
                // For SuperAdmin, we ensure it always has ALL permissions
                if (role.Name == "SuperAdmin")
                {
                    var allPermissions = await GetAllPermissions();
                    permissionIds = allPermissions.Select(p => p.PermissionId);
                }

                // For Admin, ensure it has all but system admin permissions
                if (role.Name == "Admin")
                {
                    var allPermissions = await GetAllPermissions();
                    // Filter out system admin permissions if they're not explicitly included
                    var systemPermissions = allPermissions.Where(p => p.Module == "SystemAdministration").Select(p => p.PermissionId);
                    var adminPermissions = allPermissions.Where(p => p.Module != "SystemAdministration").Select(p => p.PermissionId);

                    // Merge explicitly provided system permissions with all admin permissions
                    var explicitSystemPermissions = permissionIds.Intersect(systemPermissions);
                    permissionIds = adminPermissions.Union(explicitSystemPermissions);
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

        public async Task<bool> AssignDirectPermissionsToUser(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

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
                    foreach (var permissionId in permissionIds)
                    {
                        _context.UserPermissions.Add(new UserPermission
                        {
                            UserId = userId,
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

        public async Task<IList<Claim>> GetUserPermissionClaims(string userId)
        {
            var permissions = await GetAllUserPermissions(userId);
            return permissions
                .Select(p => new Claim("Permission", p.Code))
                .ToList();
        }

        public async Task RemovePermissionsFromUser(string userId, List<int> permissionIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == user.Id && permissionIds.Contains(up.PermissionId))
                .ToListAsync();

            _context.UserPermissions.RemoveRange(userPermissions);

            await _context.SaveChangesAsync();
        }

        public async Task SeedDefaultPermissions()
        {
            // Check if permissions already exist
            if (await _context.Permissions.AnyAsync())
                return;

            // User Management Permissions
            var permissions = new List<Permission>
            {
                new Permission { Name = "View Users", Code = "USERS.VIEW", Module = "UserManagement", Description = "Can view user list", IsActive = true },
                new Permission { Name = "Create Users", Code = "USERS.CREATE", Module = "UserManagement", Description = "Can create new users", IsActive = true },
                new Permission { Name = "Edit Users", Code = "USERS.EDIT", Module = "UserManagement", Description = "Can edit existing users", IsActive = true },
                new Permission { Name = "Delete Users", Code = "USERS.DELETE", Module = "UserManagement", Description = "Can delete users", IsActive = true },
                
                // Role Management Permissions
                new Permission { Name = "View Roles", Code = "ROLES.VIEW", Module = "RoleManagement", Description = "Can view role list", IsActive = true },
                new Permission { Name = "Create Roles", Code = "ROLES.CREATE", Module = "RoleManagement", Description = "Can create new roles", IsActive = true },
                new Permission { Name = "Edit Roles", Code = "ROLES.EDIT", Module = "RoleManagement", Description = "Can edit existing roles", IsActive = true },
                new Permission { Name = "Delete Roles", Code = "ROLES.DELETE", Module = "RoleManagement", Description = "Can delete roles", IsActive = true },
                
                // Employee Management Permissions
                new Permission { Name = "View Employees", Code = "EMPLOYEES.VIEW", Module = "EmployeeManagement", Description = "Can view employee list", IsActive = true },
                new Permission { Name = "Create Employees", Code = "EMPLOYEES.CREATE", Module = "EmployeeManagement", Description = "Can create new employees", IsActive = true },
                new Permission { Name = "Edit Employees", Code = "EMPLOYEES.EDIT", Module = "EmployeeManagement", Description = "Can edit existing employees", IsActive = true },
                new Permission { Name = "Delete Employees", Code = "EMPLOYEES.DELETE", Module = "EmployeeManagement", Description = "Can delete employees", IsActive = true },
                
                // Department Management Permissions
                new Permission { Name = "View Departments", Code = "DEPARTMENTS.VIEW", Module = "DepartmentManagement", Description = "Can view department list", IsActive = true },
                new Permission { Name = "Create Departments", Code = "DEPARTMENTS.CREATE", Module = "DepartmentManagement", Description = "Can create new departments", IsActive = true },
                new Permission { Name = "Edit Departments", Code = "DEPARTMENTS.EDIT", Module = "DepartmentManagement", Description = "Can edit existing departments", IsActive = true },
                new Permission { Name = "Delete Departments", Code = "DEPARTMENTS.DELETE", Module = "DepartmentManagement", Description = "Can delete departments", IsActive = true },
                
                // Permission Management
                new Permission { Name = "View Permissions", Code = "PERMISSIONS.VIEW", Module = "PermissionManagement", Description = "Can view permission list", IsActive = true },
                new Permission { Name = "Assign Permissions", Code = "PERMISSIONS.ASSIGN", Module = "PermissionManagement", Description = "Can assign permissions to roles", IsActive = true },
                
                // System Administration
                new Permission { Name = "System Settings", Code = "SYSTEM.SETTINGS", Module = "SystemAdministration", Description = "Can change system settings", IsActive = true },
                new Permission { Name = "View Audit Logs", Code = "SYSTEM.AUDIT", Module = "SystemAdministration", Description = "Can view system audit logs", IsActive = true }
            };

            _context.Permissions.AddRange(permissions);
            await _context.SaveChangesAsync();
        }
    }
}
