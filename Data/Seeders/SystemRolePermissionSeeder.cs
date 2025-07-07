using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class SystemRolePermissionSeeder : ISeeder
    {
        public int Order => 3;

        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.RolePermissions.AnyAsync())
                return; // Already seeded

            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "SUPERADMIN");
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "ADMIN");

            // Get existing roles and permissions
            var systemRoles = await context.Roles.ToListAsync();
            var permissions = await context.Permissions.ToListAsync();

            if (!systemRoles.Any() || !permissions.Any())
                return;

            var rolePermissions = new List<RolePermission>();

            foreach (var role in systemRoles)
            {
                var assignablePermissions = new List<int>();
                switch (role.NormalizedName)
                {
                    case "SUPERADMIN":
                    case "ADMIN":
                        assignablePermissions = permissions
                            .Select(p => p.PermissionId).ToList();
                        break;
                    case "SUPPORT":
                        assignablePermissions = permissions
                            .Where(p => p.Name.Contains("Support", StringComparison.OrdinalIgnoreCase))
                            .Select(p => p.PermissionId)
                            .ToList();
                        break;
                }

                // Create RolePermission objects for each permission assigned to this role
                foreach (var permissionId in assignablePermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId,
                        CreatedBy = "Seeder",
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false,
                    });
                }
            }
        }
    }
}
