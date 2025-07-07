using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Data;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class SystemPermissionSeeder : ISeeder
    {
        public int Order => 2;

        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return; // Already seeded

            var systemPermissions = new List<Permission>
            { 
                // Tenant Management
                new Permission { PermissionId = 1, Name = "Tenants Module", Code = "TENANTS.MODULE", Description = "Can access tenant management module", IsActive = true },
                new Permission { PermissionId = 2, Name = "View Tenants", Code = "TENANTS.VIEW", Description = "Can view tenant list" },
                new Permission { PermissionId = 3, Name = "Create Tenants", Code = "TENANTS.CREATE", Description = "Can create new tenants" },
                new Permission { PermissionId = 4, Name = "Edit Tenants", Code = "TENANTS.EDIT", Description = "Can edit tenant details" },
                new Permission { PermissionId = 5, Name = "Delete Tenants", Code = "TENANTS.DELETE", Description = "Can delete tenants" },

                // Dashboard
                new Permission { PermissionId = 6, Name = "View Dashboard", Code = "DASHBOARD.VIEW", Description = "Can access and view dashboard", IsActive = true },

                // Role Management
                new Permission { PermissionId = 7, Name = "Roles Module", Code = "ROLES.MODULE", Description = "Can access roles management module", IsActive = true },
                new Permission { PermissionId = 8, Name = "View Roles", Code = "ROLES.VIEW", Description = "Can view role list" },
                new Permission { PermissionId = 9, Name = "Create Roles", Code = "ROLES.CREATE", Description = "Can create new roles" },
                new Permission { PermissionId = 10, Name = "Edit Roles", Code = "ROLES.EDIT", Description = "Can edit existing roles" },
                new Permission { PermissionId = 11, Name = "Delete Roles", Code = "ROLES.DELETE", Description = "Can delete roles" },

                // Permission Management
                new Permission { PermissionId = 12, Name = "Permissions Module", Code = "PERMISSIONS.MODULE", Description = "Can access permissions management module", IsActive = true },
                new Permission { PermissionId = 13, Name = "View Permissions", Code = "PERMISSIONS.VIEW", Description = "Can view permission list", IsActive = true },
                new Permission { PermissionId = 14, Name = "Create Permissions", Code = "PERMISSIONS.CREATE", Description = "Can create permissions", IsActive = true },
                new Permission { PermissionId = 15, Name = "Assign Permissions", Code = "PERMISSIONS.ASSIGN", Description = "Can assign existing permissions to role/s" },
                new Permission { PermissionId = 16, Name = "Delete Permissions", Code = "PERMISSIONS.DELETE", Description = "Can delete permissions" },

                // Billing Management
                new Permission { PermissionId = 17, Name = "Platform Billing Module", Code = "PLATFORM_BILLING.MODULE", Description = "Can access platform billing module", IsActive = true },
                new Permission { PermissionId = 18, Name = "View Client Billing", Code = "PLATFORM_BILLING.VIEW", Description = "Can view platform billing" },
                new Permission { PermissionId = 19, Name = "Manage Client Billing", Code = "PLATFORM_BILLING.MANAGE", Description = "Can manage client billing and subscriptions" },
                new Permission { PermissionId = 20, Name = "Manage Client Invoices", Code = "PLATFORM_BILLING.MANAGE_INVOICES", Description = "Can view, create, manage, and export client billing invoices" },
                new Permission { PermissionId = 21, Name = "Manage Client Payments", Code = "PLATFORM_BILLING.MANAGE_PAYMENTS", Description = "Can view, manage, and process client payments and deductions." },

                // Support Management
                new Permission { PermissionId = 22, Name = "Support Module", Code = "SUPPORT.MODULE", Description = "Can access support management module", IsActive = true },
                new Permission { PermissionId = 23, Name = "View Support Tickets", Code = "SUPPORT.VIEW_TICKETS", Description = "Can view support tickets" },
                new Permission { PermissionId = 24, Name = "Create Support Tickets", Code = "SUPPORT.CREATE_TICKETS", Description = "Can create new support tickets" },
                new Permission { PermissionId = 25, Name = "Edit Support Tickets", Code = "SUPPORT.EDIT_TICKETS", Description = "Can edit existing support tickets" },
                new Permission { PermissionId = 26, Name = "Delete Support Tickets", Code = "SUPPORT.DELETE_TICKETS", Description = "Can soft-delete support tickets" },

                // Audit Logs
                new Permission { PermissionId = 27, Name = "Audit Logs Module", Code = "AUDIT.MODULE", Description = "Can access audit logs module", IsActive = true },
                new Permission { PermissionId = 28, Name = "View Audit Logs", Code = "AUDIT.VIEW", Description = "Can view audit logs", IsActive = true },
                new Permission { PermissionId = 29, Name = "Export Audit Logs", Code = "AUDIT.EXPORT", Description = "Can export audit logs to CSV", IsActive = true },

                // User Management
                new Permission { PermissionId = 30, Name = "User Module", Code = "USERS.MODULE", Description = "Can access user management module", IsActive = true },
                new Permission { PermissionId = 31, Name = "View Users", Code = "USERS.VIEW", Description = "Can view user list" },
                new Permission { PermissionId = 32, Name = "View User Profile", Code = "USERS.PROFILE", Description = "Can view user own profile" },
                new Permission { PermissionId = 33, Name = "Create Users", Code = "USERS.CREATE", Description = "Can create new users" },
                new Permission { PermissionId = 34, Name = "Edit Users", Code = "USERS.EDIT", Description = "Can edit existing users" },
                new Permission { PermissionId = 35, Name = "Delete Users", Code = "USERS.DELETE", Description = "Can soft-delete users" },

                // Employee Management
                new Permission { PermissionId = 36, Name = "Employee Module", Code = "EMPLOYEES.MODULE", Description = "Can access employee management module", IsActive = true },
                new Permission { PermissionId = 37, Name = "View Employees", Code = "EMPLOYEES.VIEW", Description = "Can view employee list", IsActive = true },
                new Permission { PermissionId = 38, Name = "Create Employees", Code = "EMPLOYEES.CREATE", Description = "Can create new employees", IsActive = true },
                new Permission { PermissionId = 39, Name = "Edit Employees", Code = "EMPLOYEES.EDIT", Description = "Can edit existing employees", IsActive = true },
                new Permission { PermissionId = 40, Name = "Delete Employees", Code = "EMPLOYEES.DELETE", Description = "Can soft-delete employees", IsActive = true },

                // Organization Management
                new Permission { PermissionId = 41, Name = "Organization Module", Code = "ORGANIZATION.MODULE", Description = "Can access organization management module", IsActive = true },
                new Permission { PermissionId = 42, Name = "View Organization", Code = "ORGANIZATION.VIEW", Description = "Can view organization list", IsActive = true },
                new Permission { PermissionId = 43, Name = "Create Organization", Code = "ORGANIZATION.CREATE", Description = "Can create new organization", IsActive = true },
                new Permission { PermissionId = 44, Name = "Edit Organization", Code = "ORGANIZATION.EDIT", Description = "Can edit existing organization", IsActive = true },
                new Permission { PermissionId = 45, Name = "Delete Organization", Code = "ORGANIZATION.DELETE", Description = "Can soft-delete organization", IsActive = true },

                // Department Management
                new Permission { PermissionId = 46, Name = "Departments Module", Code = "DEPARTMENTS.MODULE", Description = "Can access department management module", IsActive = true },
                new Permission { PermissionId = 47, Name = "View Departments", Code = "DEPARTMENTS.VIEW", Description = "Can view department list", IsActive = true },
                new Permission { PermissionId = 48, Name = "Create Departments", Code = "DEPARTMENTS.CREATE", Description = "Can create new departments", IsActive = true },
                new Permission { PermissionId = 49, Name = "Edit Departments", Code = "DEPARTMENTS.EDIT", Description = "Can edit existing departments", IsActive = true },
                new Permission { PermissionId = 50, Name = "Delete Departments", Code = "DEPARTMENTS.DELETE", Description = "Can delete departments", IsActive = true },

                // Team Management
                new Permission { PermissionId = 51, Name = "Teams Module", Code = "TEAMS.MODULE", Description = "Can access team management module", IsActive = true },
                new Permission { PermissionId = 52, Name = "View Teams", Code = "TEAMS.VIEW", Description = "Can view team list", IsActive = true },
                new Permission { PermissionId = 53, Name = "Create Teams", Code = "TEAMS.CREATE", Description = "Can create new teams", IsActive = true },
                new Permission { PermissionId = 54, Name = "Edit Teams", Code = "TEAMS.EDIT", Description = "Can edit existing teams", IsActive = true },
                new Permission { PermissionId = 55, Name = "Delete Teams", Code = "TEAMS.DELETE", Description = "Can soft-delete teams", IsActive = true },

                // Timesheet
                new Permission { PermissionId = 56, Name = "Timesheets Module", Code = "TIMESHEETS.MODULE", Description = "Can access timesheets module", IsActive = true },
                new Permission { PermissionId = 57, Name = "View Timesheets", Code = "TIMESHEETS.VIEW", Description = "Can view timesheets record" },
                new Permission { PermissionId = 58, Name = "View Team Timesheets", Code = "TIMESHEETS.VIEW_TEAM", Description = "Can view team timesheets record" },
                new Permission { PermissionId = 59, Name = "Create Timesheets", Code = "TIMESHEETS.CREATE", Description = "Can create new timesheet record" },
                new Permission { PermissionId = 60, Name = "Edit Timesheets", Code = "TIMESHEETS.EDIT", Description = "Can edit existing timesheet record" },
                new Permission { PermissionId = 61, Name = "Delete Timesheets", Code = "TIMESHEETS.DELETE", Description = "Can soft-delete timesheet record" },

                // Payslip (Employee Payroll)
                new Permission { PermissionId = 62, Name = "View Payslips", Code = "PAYSLIPS.VIEW_SELF", Description = "Can view own payslip records" },

                // Employee Profile
                new Permission { PermissionId = 63, Name = "Employee Profile", Code = "EMPLOYEES.PROFILE", Description = "Can view and edit own employee profile", IsActive = true },

                // Time-Attendance Management
                new Permission { PermissionId = 64, Name = "Time-Attendance Module", Code = "TIME_ATTENDANCE.MODULE", Description = "Can access time and attendance management module", IsActive = true },
                new Permission { PermissionId = 65, Name = "View Time-Attendance", Code = "TIME_ATTENDANCE.VIEW", Description = "Can view time and attendance records" },
                new Permission { PermissionId = 66, Name = "Update Time-Attendance", Code = "TIME_ATTENDANCE.UPDATE", Description = "Can update time and attendance records" },

                // Employee Lifecycle Management
                new Permission { PermissionId = 67, Name = "Employee Lifecycle Module", Code = "EMPLOYEE_LIFECYCLE.MODULE", Description = "Can access employee lifecycle module", IsActive = true },
                // Recruitment Permissions
                new Permission { PermissionId = 68, Name = "Recruitment Module", Code = "RECRUITMENT.MODULE", Description = "Access to recruitment module", IsActive = true },
                new Permission { PermissionId = 69, Name = "View Job Postings", Code = "RECRUITMENT.JOB_POSTINGS.VIEW", Description = "View job postings and requisitions", IsActive = true },
                new Permission { PermissionId = 70, Name = "Create Job Postings", Code = "RECRUITMENT.JOB_POSTINGS.CREATE", Description = "Create new job postings and requisitions", IsActive = true },
                new Permission { PermissionId = 71, Name = "Edit Job Postings", Code = "RECRUITMENT.JOB_POSTINGS.EDIT", Description = "Edit existing job postings and requisitions", IsActive = true },
                new Permission { PermissionId = 72, Name = "Delete Job Postings", Code = "RECRUITMENT.JOB_POSTINGS.DELETE", Description = "Delete job postings and requisitions", IsActive = true },
                new Permission { PermissionId = 73, Name = "View Candidates", Code = "RECRUITMENT.CANDIDATES.VIEW", Description = "View candidate profiles and applications", IsActive = true },
                new Permission { PermissionId = 74, Name = "Manage Candidates", Code = "RECRUITMENT.CANDIDATES.MANAGE", Description = "Add, edit, and manage candidate information", IsActive = true },
                new Permission { PermissionId = 75, Name = "Schedule Interviews", Code = "RECRUITMENT.INTERVIEWS.SCHEDULE", Description = "Schedule and manage interviews", IsActive = true },
                new Permission { PermissionId = 76, Name = "Conduct Interviews", Code = "RECRUITMENT.INTERVIEWS.CONDUCT", Description = "Conduct interviews and provide feedback", IsActive = true },
                new Permission { PermissionId = 77, Name = "Make Job Offers", Code = "RECRUITMENT.OFFERS.CREATE", Description = "Create and send job offers", IsActive = true },
                new Permission { PermissionId = 78, Name = "Approve Job Offers", Code = "RECRUITMENT.OFFERS.APPROVE", Description = "Approve job offers and compensation", IsActive = true },
                // Onboarding Permissions
                new Permission { PermissionId = 79, Name = "Onboarding Module", Code = "ONBOARDING.MODULE", Description = "Access to employee onboarding module", IsActive = true },
                new Permission { PermissionId = 80, Name = "View Onboarding Tasks", Code = "ONBOARDING.TASKS.VIEW", Description = "View onboarding checklists and tasks", IsActive = true },
                new Permission { PermissionId = 81, Name = "Manage Onboarding Tasks", Code = "ONBOARDING.TASKS.MANAGE", Description = "Create and manage onboarding checklists", IsActive = true },
                new Permission { PermissionId = 82, Name = "Assign Onboarding Tasks", Code = "ONBOARDING.TASKS.ASSIGN", Description = "Assign onboarding tasks to employees/managers", IsActive = true },
                new Permission { PermissionId = 83, Name = "Complete Onboarding Tasks", Code = "ONBOARDING.TASKS.COMPLETE", Description = "Mark onboarding tasks as complete", IsActive = true },
                new Permission { PermissionId = 84, Name = "View New Hires", Code = "ONBOARDING.NEW_HIRES.VIEW", Description = "View new hire information and progress", IsActive = true },
                new Permission { PermissionId = 85, Name = "Manage New Hires", Code = "ONBOARDING.NEW_HIRES.MANAGE", Description = "Manage new hire onboarding process", IsActive = true },
                new Permission { PermissionId = 86, Name = "Generate Onboarding Reports", Code = "ONBOARDING.REPORTS.GENERATE", Description = "Generate onboarding progress and completion reports", IsActive = true },
                // Offboarding Permissions
                new Permission { PermissionId = 87, Name = "Offboarding Module", Code = "OFFBOARDING.MODULE", Description = "Access to employee offboarding module", IsActive = true },
                new Permission { PermissionId = 88, Name = "Initiate Offboarding", Code = "OFFBOARDING.PROCESS.INITIATE", Description = "Start offboarding process for employees", IsActive = true },
                new Permission { PermissionId = 89, Name = "View Offboarding Tasks", Code = "OFFBOARDING.TASKS.VIEW", Description = "View offboarding checklists and tasks", IsActive = true },
                new Permission { PermissionId = 90, Name = "Manage Offboarding Tasks", Code = "OFFBOARDING.TASKS.MANAGE", Description = "Create and manage offboarding checklists", IsActive = true },
                new Permission { PermissionId = 91, Name = "Complete Offboarding Tasks", Code = "OFFBOARDING.TASKS.COMPLETE", Description = "Mark offboarding tasks as complete", IsActive = true },
                new Permission { PermissionId = 92, Name = "Conduct Exit Interviews", Code = "OFFBOARDING.EXIT_INTERVIEWS.CONDUCT", Description = "Schedule and conduct exit interviews", IsActive = true },
                new Permission { PermissionId = 93, Name = "View Exit Interview Data", Code = "OFFBOARDING.EXIT_INTERVIEWS.VIEW", Description = "View exit interview responses and analytics", IsActive = true },
                new Permission { PermissionId = 94, Name = "Asset Recovery", Code = "OFFBOARDING.ASSETS.RECOVER", Description = "Manage company asset recovery during offboarding", IsActive = true },
                new Permission { PermissionId = 95, Name = "Final Approval", Code = "OFFBOARDING.APPROVAL.FINAL", Description = "Provide final approval for employee offboarding", IsActive = true },

                // Compensation and Benefits
                new Permission { PermissionId = 96, Name = "Compensation & Benefits Module", Code = "COMPENSATION_BENEFITS.MODULE", Description = "Access to compensation and benefits management system", IsActive = true },
                new Permission { PermissionId = 97, Name = "Compensation Setup", Code = "COMPENSATION.SETUP", Description = "Access to manage compensation configurations such as grades, pay rules, and benefits setup", IsActive =true },
                // Payroll Management
                new Permission { PermissionId = 98, Name = "Payroll Module", Code = "PAYROLL.MODULE", Description = "Access to payroll processing system", IsActive = true },
                new Permission { PermissionId = 99, Name = "View Own Payroll", Code = "PAYROLL.VIEW_SELF", Description = "View own payroll records and pay stubs", IsActive = true },
                new Permission { PermissionId = 100, Name = "View All Payroll", Code = "PAYROLL.VIEW_ALL", Description = "View all employee payroll records", IsActive = true },
                new Permission { PermissionId = 101, Name = "Process Payroll", Code = "PAYROLL.PROCESS", Description = "Process and calculate payroll", IsActive = true },
                new Permission { PermissionId = 102, Name = "Edit Payroll Records", Code = "PAYROLL.EDIT", Description = "Edit payroll calculations and records", IsActive = true },
                new Permission { PermissionId = 103, Name = "Approve Payroll", Code = "PAYROLL.APPROVE", Description = "Approve payroll before disbursement", IsActive = true },
                // Benefits Management
                new Permission { PermissionId = 104, Name = "Benefits Module", Code = "BENEFITS.MODULE", Description = "Access to employee benefits management", IsActive = true },
                new Permission { PermissionId = 105, Name = "View Own Benefits", Code = "BENEFITS.VIEW_SELF", Description = "View own benefits enrollment and details", IsActive = true },
                new Permission { PermissionId = 106, Name = "View Team Benefits", Code = "BENEFITS.VIEW_TEAM", Description = "View benefits information for managed team", IsActive = true },
                new Permission { PermissionId = 107, Name = "View All Benefits", Code = "BENEFITS.VIEW_ALL", Description = "View all employee benefits information", IsActive = true },
                new Permission { PermissionId = 108, Name = "Enroll in Benefits", Code = "BENEFITS.ENROLL", Description = "Enroll in available benefits programs", IsActive = true },
                new Permission { PermissionId = 109, Name = "Manage Benefits Enrollment", Code = "BENEFITS.ENROLLMENT.MANAGE", Description = "Manage benefits enrollment for employees", IsActive = true },
                new Permission { PermissionId = 110, Name = "Configure Benefits Plans", Code = "BENEFITS.PLANS.CONFIGURE", Description = "Configure and manage benefits plans", IsActive = true },
                new Permission { PermissionId = 111, Name = "Process Benefits Claims", Code = "BENEFITS.CLAIMS.PROCESS", Description = "Process benefits claims and reimbursements", IsActive = true },
                // Deductions Management
                new Permission { PermissionId = 112, Name = "View Own Deductions", Code = "DEDUCTIONS.VIEW_SELF", Description = "View own salary deductions", IsActive = true },
                new Permission { PermissionId = 113, Name = "View All Deductions", Code = "DEDUCTIONS.VIEW_ALL", Description = "View all employee deductions", IsActive = true },
                new Permission { PermissionId = 114, Name = "Manage Deductions", Code = "DEDUCTIONS.MANAGE", Description = "Create and manage salary deductions", IsActive = true },
                new Permission { PermissionId = 115, Name = "Apply Deductions", Code = "DEDUCTIONS.APPLY", Description = "Apply deductions to employee payroll", IsActive = true },
                // Bonuses and Incentives
                new Permission { PermissionId = 116, Name = "View Own Bonuses", Code = "BONUSES.VIEW_SELF", Description = "View own bonuses and incentives", IsActive = true },
                new Permission { PermissionId = 117, Name = "View All Bonuses", Code = "BONUSES.VIEW_ALL", Description = "View all employee bonuses and incentives", IsActive = true },
                new Permission { PermissionId = 118, Name = "Create Bonuses", Code = "BONUSES.CREATE", Description = "Create bonus payments and incentives", IsActive = true },
                new Permission { PermissionId = 119, Name = "Approve Bonuses", Code = "BONUSES.APPROVE", Description = "Approve bonus payments", IsActive = true },
                // Tax Management
                new Permission { PermissionId = 120, Name = "Tax Management", Code = "TAX.MANAGE", Description = "Manage tax calculations and compliance", IsActive = true },
                new Permission { PermissionId = 121, Name = "Generate Tax Reports", Code = "TAX.REPORTS", Description = "Generate tax reports and documents", IsActive = true },
                new Permission { PermissionId = 122, Name = "View Tax Information", Code = "TAX.VIEW", Description = "View employee tax information", IsActive = true },

                // Training and Development
                new Permission { PermissionId = 123, Name = "Training & Development", Code = "TRAINING_DEVELOPMENT.MODULE", Description = "Can access training and development module", IsActive = true },

                // Compliance
                new Permission { PermissionId = 124, Name = "Compliance", Code = "COMPLIANCE.MODULE", Description = "Can access HR compliance module", IsActive = true },

                // Tickets
                new Permission { PermissionId = 125, Name = "Tickets Module", Code = "TICKETS.MODULE", Description = "Can access support ticket management module", IsActive = true },
                new Permission { PermissionId = 126, Name = "View Tickets", Code = "TICKETS.VIEW", Description = "Can view support tickets" },
                new Permission { PermissionId = 127, Name = "Create Tickets", Code = "TICKETS.CREATE", Description = "Can create new support tickets" },
                new Permission { PermissionId = 128, Name = "Edit Tickets", Code = "TICKETS.EDIT", Description = "Can edit existing support tickets" },
                new Permission { PermissionId = 129, Name = "Delete Tickets", Code = "TICKETS.DELETE", Description = "Can soft-delete support tickets" },

                // System Administration
                new Permission { PermissionId = 130, Name = "System Administration", Code = "SYSTEM.ADMIN", Description = "Can access system administration functions", IsActive = true },
                new Permission { PermissionId = 131, Name = "System Settings", Code = "SYSTEM.SETTINGS", Description = "Can change system settings", IsActive = true },
                new Permission { PermissionId = 132, Name = "View Audit Logs", Code = "SYSTEM.AUDIT", Description = "Can view system audit logs", IsActive = true },
                new Permission { PermissionId = 133, Name = "System Security", Code = "SYSTEM.SECURITY", Description = "Can manage security settings" }
            };

            await context.Permissions.AddRangeAsync(systemPermissions);
            await context.SaveChangesAsync();
        }
    }
}
