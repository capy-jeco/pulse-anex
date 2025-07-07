using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Security;
using System.Data;

namespace portal_agile.Data.Seeders
{
    public class SystemRoleSeeder : ISeeder
    {
        public int Order => 1;

        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return; // Already seeded

            var coreTenant = await context.Tenants.FirstOrDefaultAsync(t => t.IsSystem);

            var systemRoles = new List<Role>()
            {
                new Role
                {
                    TenantId = 1, // Hard Coded for Anex-Core
                    Name = "Super Admin",
                    NormalizedName = "SUPERADMIN",
                    Description = "Super Administrator with full access to all system functions",
                    IsSystemRole = true,
                },
                new Role
                {
                    TenantId = 1,
                    Name = "Administrator",
                    NormalizedName = "ADMIN",
                    Description = "Administrator with access to most system functions",
                    IsSystemRole = true,
                },
                new Role
                {
                    TenantId = 1,
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "Regular employee with limited access to personal information",
                    IsSystemRole = false,
                }
            };

            await context.Roles.AddRangeAsync(systemRoles);
            await context.SaveChangesAsync();
        }
    }
}
