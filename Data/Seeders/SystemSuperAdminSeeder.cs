using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Models;
using portal_agile.Security;
using portal_agile.Services;

namespace portal_agile.Data.Seeders
{
    public class SystemSuperAdminSeeder : ISeeder
    {
        public int Order => 4;

        private readonly IServiceProvider _serviceProvider;

        public SystemSuperAdminSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SeedAsync(AppDbContext context)
        {
            using var scope = _serviceProvider.CreateScope();

            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "SUPERADMIN");

            if (superAdminRole == null)
                throw new InvalidOperationException("Super Admin role not found. Ensure SystemRoleSeeder has run first.");

            if (!context.Users.IgnoreQueryFilters().Any(u => u.Email == "superadmin@anex.com"))
            {
                var superAdmin = new User
                {
                    TenantId = 1,
                    UserName = "superadmin@anex.com",
                    Email = "superadmin@anex.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                // Create the user with password
                var createResult = await userManager.CreateAsync(superAdmin, "SuperAdmin12345!"); // In production, use secure password management
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
                else
                {
                    // Log or throw the errors to help you debug
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create SuperAdmin user: {errors}");
                }
            }
        }
    }
}
