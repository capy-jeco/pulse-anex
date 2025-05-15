using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-one relationship between ApplicationUser and Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure self-referencing relationship for Employee (Manager/DirectReports)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .IsRequired(false) // Manager is optional
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure one-to-many relationship between Department and Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure relationships for Permissions
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.DirectPermissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraints
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Optional: Configure table names to override convention
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Permission>().ToTable("Permissions");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermissions");
            modelBuilder.Entity<UserPermission>().ToTable("UserPermissions");

            // Seed default permissions
            SeedPermissions(modelBuilder);

            // Seed default roles (Admin and SuperAdmin)
            SeedDefaultRoles(modelBuilder);

            // Seed data for departments (optional)
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "Human Resources", Description = "HR Department" },
                new Department { DepartmentId = 2, Name = "Information Technology", Description = "IT Department" },
                new Department { DepartmentId = 3, Name = "Finance", Description = "Finance Department" }
            );

        }

        private void SeedPermissions(ModelBuilder modelBuilder)
        {
            // User Management Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 1, Name = "View Users", Code = "USERS.VIEW", Module = "UserManagement", Description = "Can view user list", IsActive = true },
                new Permission { PermissionId = 2, Name = "Create Users", Code = "USERS.CREATE", Module = "UserManagement", Description = "Can create new users", IsActive = true },
                new Permission { PermissionId = 3, Name = "Edit Users", Code = "USERS.EDIT", Module = "UserManagement", Description = "Can edit existing users", IsActive = true },
                new Permission { PermissionId = 4, Name = "Delete Users", Code = "USERS.DELETE", Module = "UserManagement", Description = "Can delete users", IsActive = true }
            );

            // Role Management Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 5, Name = "View Roles", Code = "ROLES.VIEW", Module = "RoleManagement", Description = "Can view role list", IsActive = true },
                new Permission { PermissionId = 6, Name = "Create Roles", Code = "ROLES.CREATE", Module = "RoleManagement", Description = "Can create new roles", IsActive = true },
                new Permission { PermissionId = 7, Name = "Edit Roles", Code = "ROLES.EDIT", Module = "RoleManagement", Description = "Can edit existing roles", IsActive = true },
                new Permission { PermissionId = 8, Name = "Delete Roles", Code = "ROLES.DELETE", Module = "RoleManagement", Description = "Can delete roles", IsActive = true }
            );

            // Employee Management Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 9, Name = "View Employees", Code = "EMPLOYEES.VIEW", Module = "EmployeeManagement", Description = "Can view employee list", IsActive = true },
                new Permission { PermissionId = 10, Name = "Create Employees", Code = "EMPLOYEES.CREATE", Module = "EmployeeManagement", Description = "Can create new employees", IsActive = true },
                new Permission { PermissionId = 11, Name = "Edit Employees", Code = "EMPLOYEES.EDIT", Module = "EmployeeManagement", Description = "Can edit existing employees", IsActive = true },
                new Permission { PermissionId = 12, Name = "Delete Employees", Code = "EMPLOYEES.DELETE", Module = "EmployeeManagement", Description = "Can delete employees", IsActive = true }
            );

            // Department Management Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 13, Name = "View Departments", Code = "DEPARTMENTS.VIEW", Module = "DepartmentManagement", Description = "Can view department list", IsActive = true },
                new Permission { PermissionId = 14, Name = "Create Departments", Code = "DEPARTMENTS.CREATE", Module = "DepartmentManagement", Description = "Can create new departments", IsActive = true },
                new Permission { PermissionId = 15, Name = "Edit Departments", Code = "DEPARTMENTS.EDIT", Module = "DepartmentManagement", Description = "Can edit existing departments", IsActive = true },
                new Permission { PermissionId = 16, Name = "Delete Departments", Code = "DEPARTMENTS.DELETE", Module = "DepartmentManagement", Description = "Can delete departments", IsActive = true }
            );

            // Permission Management (Meta Permissions)
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 17, Name = "View Permissions", Code = "PERMISSIONS.VIEW", Module = "PermissionManagement", Description = "Can view permission list", IsActive = true },
                new Permission { PermissionId = 18, Name = "Assign Permissions", Code = "PERMISSIONS.ASSIGN", Module = "PermissionManagement", Description = "Can assign permissions to roles", IsActive = true }
            );

            // System Administration Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 19, Name = "System Settings", Code = "SYSTEM.SETTINGS", Module = "SystemAdministration", Description = "Can change system settings", IsActive = true },
                new Permission { PermissionId = 20, Name = "View Audit Logs", Code = "SYSTEM.AUDIT", Module = "SystemAdministration", Description = "Can view system audit logs", IsActive = true }
            );
        }

        private void SeedDefaultRoles(ModelBuilder modelBuilder)
        {
            // Define Admin and SuperAdmin roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = "1",
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Description = "Super Administrator with full access to all system functions",
                    IsSystemRole = true,
                    CreatedDate = new DateTime(2023, 1, 1)
                },
                new Role
                {
                    Id = "2",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrator with access to most system functions",
                    IsSystemRole = true,
                    CreatedDate = new DateTime(2023, 1, 1)
                }
            );

            // Assign ALL permissions to SuperAdmin
            for (int i = 1; i <= 20; i++)
            {
                modelBuilder.Entity<RolePermission>().HasData(
                    new RolePermission
                    {
                        RolePermissionId = i + 20, // offset to avoid duplicate PKs
                        RoleId = "1", // SuperAdmin
                        PermissionId = i,
                        CreatedDate = new DateTime(2023, 1, 1),
                        CreatedBy = "System"
                    }
                );
            }

            // Assign permissions to Admin role (all except system administration)
            for (int i = 1; i <= 18; i++)
            {
                modelBuilder.Entity<RolePermission>().HasData(
                    new RolePermission
                    {
                        RolePermissionId = i,
                        RoleId = "2", // Admin
                        PermissionId = i,
                        CreatedDate = new DateTime(2023, 1, 1),
                        CreatedBy = "System"
                    }
                );
            }
        }

        public async Task EnsureSuperAdminCreatedAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // Check if SuperAdmin role exists
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                // Create SuperAdmin role if it doesn't exist (should already be seeded, but just in case)
                var superAdminRole = new Role("SuperAdmin")
                {
                    Description = "Super Administrator with full access to all system functions",
                    IsSystemRole = true
                };
                await roleManager.CreateAsync(superAdminRole);
            }

            // Create a default SuperAdmin user if none exists
            if (!userManager.Users.Any(u => u.UserName == "superadmin@example.com"))
            {
                var superAdmin = new User
                {
                    UserName = "superadmin@example.com",
                    Email = "superadmin@example.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

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
