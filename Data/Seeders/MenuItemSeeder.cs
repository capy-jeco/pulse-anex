using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;
using System.Reflection;

namespace portal_agile.Data.Seeders
{
    public class MenuItemSeeder : ISeeder
    {
        public int Order => 6; // Ensure this runs after ModuleSeeder

        public async Task SeedAsync(AppDbContext context)
        {
            if (context.MenuItems.Any())
                return; // Already seeded

            var modules = await context.Modules.ToListAsync();

            var menuItems = new List<MenuItem>()
            {
                // Level 0 Menu Items (Top Level)
                new MenuItem
                {
                    Id = 1,
                    Label = "Dashboard",
                    Icon = "bar-chart-line",
                    Route = "/dashboard",
                    ModuleId = modules.Where(m => m.ModuleName == "Dashboard").First().Id, // Will be replaced with actual module ID during seeding
                    MenuLevel = 0,
                    SortOrder = 0,
                    RequiredPermission = "DASHBOARD.VIEW"
                },
                new MenuItem
                {
                    Id = 2,
                    Label = "Tenants",
                    Icon = "database",
                    ModuleId = modules.Where(m => m.ModuleName == "TenantManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 1,
                    RequiredPermission = "TENANTS.MODULE"
                },
                new MenuItem
                {
                    Id = 3,
                    Label = "Roles & Permissions",
                    Icon = "shield-check",
                    ModuleId = modules.Where(m => m.ModuleName == "RolesPermissions").First().Id,
                    MenuLevel = 0,
                    SortOrder = 2,
                    RequiredPermission = "PERMISSIONS.MODULE"
                },
                new MenuItem
                {
                    Id = 4,
                    Label = "Platform Billing",
                    Icon = "credit-card",
                    Route = "/platform-billing",
                    ModuleId = modules.Where(m => m.ModuleName == "PlatformBilling").First().Id,
                    MenuLevel = 0,
                    SortOrder = 3,
                    RequiredPermission = "PLATFORM_BILLING.MODULE"
                },
                new MenuItem
                {
                    Id = 5,
                    Label = "Support & Helpdesk",
                    Icon = "chat-left-dots",
                    Route = "/support",
                    ModuleId = modules.Where(m => m.ModuleName == "SupportManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 4,
                    RequiredPermission = "SUPPORT.MODULE"
                },
                new MenuItem
                {
                    Id = 6,
                    Label = "Audit Logs",
                    Icon = "file-text",
                    Route = "/audit-logs",
                    ModuleId = modules.Where(m => m.ModuleName == "AuditLogs").First().Id,
                    MenuLevel = 0,
                    SortOrder = 5,
                    RequiredPermission = "AUDIT.MODULE"
                },
                new MenuItem
                {
                    Id = 7,
                    Label = "Timesheets",
                    Icon = "clock-history",
                    Route = "/timesheets",
                    ModuleId = modules.Where(m => m.ModuleName == "TimesheetManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 6,
                    RequiredPermission = "TIMESHEETS.MODULE"
                },
                new MenuItem
                {
                    Id = 8,
                    Label = "Payslips",
                    Icon = "wallet-2",
                    Route = "/payslips",
                    ModuleId = modules.Where(m => m.ModuleName == "Payslip").First().Id,
                    MenuLevel = 0,
                    SortOrder = 7,
                    RequiredPermission = "PAYSLIPS.VIEW_SELF"
                },
                new MenuItem
                {
                    Id = 9,
                    Label = "Profile",
                    Icon = "person",
                    Route = "/profile",
                    ModuleId = modules.Where(m => m.ModuleName == "ProfileManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 8,
                    RequiredPermission = "EMPLOYEES.MODULE"
                },
                new MenuItem
                {
                    Id = 10,
                    Label = "Employees",
                    Icon = "person-rolodex",
                    ModuleId = modules.Where(m => m.ModuleName == "EmployeeManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 9,
                    RequiredPermission = "EMPLOYEES.MODULE"
                },
                new MenuItem
                {
                    Id = 11,
                    Label = "Users",
                    Icon = "people",
                    ModuleId = modules.Where(m => m.ModuleName == "UserManagement").First().Id,
                    MenuLevel = 0,
                    SortOrder = 10,
                    RequiredPermission = "USERS.MODULE"
                },
                new MenuItem
                {
                    Id = 12,
                    Label = "Organization",
                    Icon = "diagram-3",
                    ModuleId = modules.Where(m => m.ModuleName == "OrganizationStructure").First().Id,
                    MenuLevel = 0,
                    SortOrder = 11,
                    RequiredPermission = "ORGANIZATION.MODULE"
                },
                new MenuItem
                {
                    Id = 13,
                    Label = "Time & Attendance",
                    Icon = "clock",
                    Route = "/time-attendance",
                    ModuleId = modules.Where(m => m.ModuleName == "TimeAttendance").First().Id,
                    MenuLevel = 0,
                    SortOrder = 12,
                    RequiredPermission = "TIME_ATTENDANCE.MODULE"
                },
                new MenuItem
                {
                    Id = 14,
                    Label = "Employee Lifecycle",
                    Icon = "person-heart",
                    Route = "/employee-lifecycle",
                    ModuleId = modules.Where(m => m.ModuleName == "EmployeeLifecycle").First().Id,
                    MenuLevel = 0,
                    SortOrder = 13,
                    RequiredPermission = "EMPLOYEE_LIFECYCLE.MODULE"
                },
                new MenuItem
                {
                    Id = 15,
                    Label = "Compensation & Benefits",
                    Icon = "cash-stack",
                    Route = "/compensation-benefits",
                    ModuleId = modules.Where(m => m.ModuleName == "CompensationBenefits").First().Id,
                    MenuLevel = 0,
                    SortOrder = 14,
                    RequiredPermission = "COMPENSATION_BENEFITS.MODULE"
                },
                new MenuItem
                {
                    Id = 16,
                    Label = "Training & Development",
                    Icon = "light-bulb",
                    Route = "/training-development",
                    ModuleId = modules.Where(m => m.ModuleName == "TrainingDevelopment").First().Id,
                    MenuLevel = 0,
                    SortOrder = 15,
                    RequiredPermission = "TRAINING_DEVELOPMENT.MODULE"
                },
                new MenuItem
                {
                    Id = 17,
                    Label = "HR Compliance",
                    Icon = "shield-exclamation",
                    Route = "/compliance",
                    ModuleId = modules.Where(m => m.ModuleName == "HRCompliance").First().Id,
                    MenuLevel = 0,
                    SortOrder = 16,
                    RequiredPermission = "COMPLIANCE.MODULE"
                },
                new MenuItem
                {
                    Id = 18,
                    Label = "Ticket & Support",
                    Icon = "chat-left-text",
                    Route = "/tickets",
                    ModuleId = modules.Where(m => m.ModuleName == "TicketSupport").First().Id,
                    MenuLevel = 0,
                    SortOrder = 17,
                    RequiredPermission = "TICKETS.MODULE"
                },

                // Level 1 Menu Items (Sub-menus)
                new MenuItem
                {
                    Id = 19,
                    Label = "Roles",
                    Route = "/roles",
                    ModuleId = modules.Where(m => m.ModuleName == "RolesPermissions").First().Id,
                    ParentId = 3,
                    MenuLevel = 1,
                    SortOrder = 1,
                    RequiredPermission = "ROLES.MODULE"
                },
                new MenuItem
                {
                    Id = 20,
                    Label = "Permissions",
                    Route = "/permissions",
                    ModuleId = modules.Where(m => m.ModuleName == "RolesPermissions").First().Id,
                    ParentId = 3,
                    MenuLevel = 1,
                    SortOrder = 2,
                    RequiredPermission = "PERMISSIONS.MODULE"
                },
                new MenuItem
                {
                    Id = 21,
                    Label = "Departments",
                    Route = "/departments",
                    ModuleId = modules.Where(m => m.ModuleName == "OrganizationStructure").First().Id, // Department module ID
                    ParentId = 12,
                    MenuLevel = 1,
                    SortOrder = 1,
                    RequiredPermission = "DEPARTMENTS.MODULE"
                },
                new MenuItem
                {
                    Id = 22,
                    Label = "Teams",
                    Route = "/teams",
                    ModuleId = modules.Where(m => m.ModuleName == "OrganizationStructure").First().Id, // Team module ID
                    ParentId = 12,
                    MenuLevel = 1,
                    SortOrder = 2,
                    RequiredPermission = "TEAMS.MODULE"
                },
                new MenuItem
                {
                    Id = 23,
                    Label = "Recruitment",
                    Route = "/recruitment",
                    ModuleId = modules.Where(m => m.ModuleName == "EmployeeLifecycle").First().Id,
                    ParentId = 14,
                    MenuLevel = 1,
                    SortOrder = 1,
                    RequiredPermission = "RECRUITMENT.MODULE"
                },
                new MenuItem
                {
                    Id = 24,
                    Label = "Onboarding",
                    Route = "/onboarding",
                    ModuleId = modules.Where(m => m.ModuleName == "EmployeeLifecycle").First().Id,
                    ParentId = 14,
                    MenuLevel = 1,
                    SortOrder = 2,
                    RequiredPermission = "ONBOARDING.MODULE"
                },
                new MenuItem
                {
                    Id = 25,
                    Label = "Offboarding",
                    Route = "/offboarding",
                    ModuleId = modules.Where(m => m.ModuleName == "EmployeeLifecycle").First().Id,
                    ParentId = 14,
                    MenuLevel = 1,
                    SortOrder = 3,
                    RequiredPermission = "OFFBOARDING.MODULE"
                }
            };

            await context.MenuItems.AddRangeAsync(menuItems);
            await context.SaveChangesAsync();
        }
    }
}
