using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class ModuleSeeder : ISeeder
    {
        public int Order => 5;

        public async Task SeedAsync(AppDbContext context)
        {
            if (context.Modules.Any())
                return; // Already seeded

            var modules = new List<Module>()
            {
                // === 1. Core System Administration (Super-Admin / 'Anex' Level) ===
                // These modules are typically accessible only by a super-administrator
                // to manage the entire multi-tenant platform itself.
                new Module
                {
                    ModuleName = "TenantManagement",
                    DisplayName = "Tenants",
                    Description = "Create, configure, and oversee tenant accounts, subscription plans, and global tenant settings."
                },
                new Module
                {
                    ModuleName = "RolesPermissions",
                    DisplayName = "Roles & Permissions",
                    Description = "Define roles and permissions globally, controlling access to HRIS functionalities."
                },
                new Module
                {
                    ModuleName = "PlatformBilling",
                    DisplayName = "Platform Billing",
                    Description = "Manage client subscriptions, invoices, and payments for the platform's multi-tenant architecture."
                },
                new Module
                {
                    ModuleName = "SupportManagement",
                    DisplayName = "Support & Helpdesk",
                    Description = "Oversee support tickets, user inquiries, and helpdesk operations across all tenants."
                },
                new Module
                {
                    ModuleName = "AuditLogs",
                    DisplayName = "Platform Audit Logs", // Clarified scope
                    Description = "Track and review system-wide activities, administrative actions, and critical data changes across all tenants."
                },

                // === 2. HRIS Modules (Tenant-Specific / 'Pulse' Level) ===
                // These modules are accessible by tenant administrators/HR users to manage their
                // specific organization's HR functions. Data is scoped per tenant.

                // Dashboard (Role-Specific)
                new Module
                {
                    ModuleName = "Dashboard",
                    DisplayName = "Dashboard",
                    Description = "A personalized overview of key metrics, pending tasks, and essential information, dynamically tailored to each user's role for quick and relevant insights."
                },

                // Employee Self-Service (ESS) Modules
                new Module
                {
                    ModuleName = "TimesheetManagement",
                    DisplayName = "Timesheets",
                    Description = "Allow employees to submit and manage their timesheets, track hours worked, and request time off."
                },
                new Module
                {
                    ModuleName = "Payslip",
                    DisplayName = "Payslips",
                    Description = "View payslips information, deductions, and tax details."
                },
                new Module
                {
                    ModuleName = "ProfileManagement",
                    DisplayName = "Profile",
                    Description = "Enable employees to manage their profiles."
                },

                // Employee & Organization Management
                new Module
                {
                    ModuleName = "EmployeeManagement",
                    DisplayName = "Employees",
                    Description = "Maintain comprehensive employee profiles, personal details, and employment history for the current tenant."
                },
                new Module
                {
                    ModuleName = "UserManagement", // This is the tenant-scoped user management
                    DisplayName = "Users",
                    Description = "Manage users, their roles, and access permissions within the current tenant."
                },
                new Module
                {
                    ModuleName = "OrganizationStructure",
                    DisplayName = "Organization",
                    Description = "Define and manage company departments, teams, job titles, and reporting lines for the current tenant."
                },

                // Time & Attendance
                new Module
                {
                    ModuleName = "TimeAttendance",
                    DisplayName = "Time & Attendance",
                    Description = "Track employee attendance, working hours, leave requests, and time-off policies for the tenant."
                },

                // Employee Lifecycle Management
                new Module
                {
                    ModuleName = "EmployeeLifecycle",
                    DisplayName = "Employee Lifecycle",
                    Description = "Streamline processes for hiring, onboarding, offboarding, including checklists and documentation within the tenant."
                },

                // 8 Compensation & Benefits
                new Module
                {
                    ModuleName = "CompensationBenefits",
                    DisplayName = "Compensation & Benefits",
                    Description = "Manage employee compensation structures, benefits enrollment, and deductions for the tenant."
                },

                // 9 Development
                new Module
                {
                    ModuleName = "TrainingDevelopment",
                    DisplayName = "Training & Development",
                    Description = "Oversee employee training programs, certifications, and skill development for the tenant."
                },

                // 10 Compliance
                new Module
                {
                    ModuleName = "HRCompliance", // Renamed for clarity
                    DisplayName = "HR Compliance",
                    Description = "Ensure adherence to labor laws, regulations, and company policies relevant to the tenant's operations."
                },
                new Module
                {
                    ModuleName = "DocumentManagement",
                    DisplayName = "Document Management",
                    Description = "Securely store and manage HR-related documents and employee files for the tenant."
                },

                // 10 Ticket-Support Modules (Tenant Level)
                new Module
                {
                    ModuleName = "TicketSupport",
                    DisplayName = "Ticket & Support",
                    Description = "Manage support tickets, user inquiries, and helpdesk operations within the current tenant."
                }
            };

            await context.Modules.AddRangeAsync(modules);
            await context.SaveChangesAsync();
        }
    }
}
