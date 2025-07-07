using Microsoft.EntityFrameworkCore;
using portal_agile.Models;

namespace portal_agile.Data.Seeders
{
    public class DepartmentSeeder
    {
        public static async Task SeedAsync(AppDbContext context, int currentTenantId)
        {
            if (await context.Departments.AnyAsync())
                return; // Already seeded for this tenant

            var departments = new[]
            {
                new Department
                {
                    Name = "Human Resources",
                    Description = "Manages recruitment, employee relations, benefits",
                },
                new Department
                {
                    Name = "Finance",
                    Description = "Handles budgets, payroll, and financial reporting",
                },
                new Department
                {
                    Name = "IT",
                    Description = "Responsible for technology infrastructure, support, and development",
                },
            };
            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();
        }
    }
}
