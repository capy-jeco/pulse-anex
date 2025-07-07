using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class MenuItemPermissionSeeder : ISeeder
    {
        public int Order => 7;

        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.MenuItemPermissions.AnyAsync())
                return; // Already seeded

            var permissions = await context.Permissions.ToListAsync();
            var menuItems = await context.MenuItems.ToListAsync();

            var menuItemPermissions = new List<MenuItemPermission>()
            {
                // Dashboard Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Dashboard").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "DASHBOARD.VIEW").First().PermissionId
                },

                // Tenant Management Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Tenants").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "TENANTS.MODULE").First().PermissionId
                },

                // Role-Permissions Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Roles & Permissions").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "PERMISSIONS.MODULE").First().PermissionId,
                },
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Roles").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "ROLES.MODULE").First().PermissionId
                },
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Permissions").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "PERMISSIONS.MODULE").First().PermissionId
                },

                // Platform Billing Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Platform Billing").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "PLATFORM_BILLING.MODULE").First().PermissionId,
                },

                // Support Management Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Support & Helpdesk").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "SUPPORT.MODULE").First().PermissionId,
                },

                // Audit Logs Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Audit Logs").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "AUDIT.MODULE").First().PermissionId,
                },

                // Timesheets Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Timesheets").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "TIMESHEETS.MODULE").First().PermissionId,
                },

                // Payslips Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Payslips").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "PAYSLIPS.VIEW_SELF").First().PermissionId,
                },

                // Profile Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Profile").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "EMPLOYEES.MODULE").First().PermissionId,
                },

                // Employees Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Employees").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "EMPLOYEES.MODULE").First().PermissionId,
                },

                // Users Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Users").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "USERS.MODULE").First().PermissionId,
                },

                // Organization Menu
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Organization").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "ORGANIZATION.MODULE").First().PermissionId,
                },

                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Departments").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "DEPARTMENTS.MODULE").First().PermissionId,
                },

                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Teams").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "TEAMS.MODULE").First().PermissionId,
                },

                // Time-Attendance
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Time & Attendance").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "TIME_ATTENDANCE.MODULE").First().PermissionId
                },

                // Employee Lifecycle
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Employee Lifecycle").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "EMPLOYEE_LIFECYCLE.MODULE").First().PermissionId
                },
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Onboarding").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "ONBOARDING.MODULE").First().PermissionId
                },
                new MenuItemPermission
                {
                    MenuItemId      = menuItems.Where(mi => mi.Label == "Offboarding").First().Id,
                    PermissionId    = permissions.Where(p => p.Code == "OFFBOARDING.MODULE").First().PermissionId
                },

                // Compensation Benefits
                new MenuItemPermission
                {
                    MenuItemId = menuItems.Where(mi => mi.Label == "Compensation & Benefits").First().Id,
                    PermissionId = permissions.Where(p => p.Code == "COMPENSATION_BENEFITS.MODULE").First().PermissionId
                },

                // Training and Development
                new MenuItemPermission
                {
                    MenuItemId = menuItems.Where(mi => mi.Label == "Training & Development").First().Id,
                    PermissionId = permissions.Where(p => p.Code == "TRAINING_DEVELOPMENT.MODULE").First().PermissionId
                },

                // HR Compliance
                new MenuItemPermission
                {
                    MenuItemId = menuItems.Where(mi => mi.Label == "Ticket & Support").First().Id,
                    PermissionId = permissions.Where(p => p.Code == "TICKETS.MODULE").First().PermissionId
                }
            };

            await context.MenuItemPermissions.AddRangeAsync(menuItemPermissions);
            await context.SaveChangesAsync();
        }
    }
}
