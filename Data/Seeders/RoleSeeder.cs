using Microsoft.EntityFrameworkCore;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class RoleSeeder
    {
        public static void Seed(ModelBuilder modelBuilder, int currentTenantId)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "Super Admin",
                    NormalizedName = "SUPERADMIN",
                    Description = "Super Administrator with full access to all system functions",
                    IsSystemRole = true,
                },
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "Administrator",
                    NormalizedName = "ADMIN",
                    Description = "Administrator with access to most system functions",
                    IsSystemRole = true,
                },
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "HR Generalist",
                    NormalizedName = "HRGEN",
                    Description = "User with access to HR-related functions",
                    IsSystemRole = false,
                },
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "HR Payroll",
                    NormalizedName = "HRPAYROLL",
                    Description = "User with access to HR-payroll functions",
                    IsSystemRole = false,
                },
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Manager with access to employee management and reporting functions",
                    IsSystemRole = false,
                },
                new Role
                {
                    TenantId = currentTenantId,
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "Regular employee with limited access to personal information",
                    IsSystemRole = false,
                }
            );
        }
    }
}
