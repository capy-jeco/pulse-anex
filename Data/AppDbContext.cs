using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Services;
using portal_agile.Data.Seeders;
using portal_agile.Models;
using portal_agile.Security;
using portal_agile.Services;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace portal_agile.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        private readonly ITenantService _tenantService;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<MenuItemPermission> MenuItemPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Tenant Configuration
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasIndex(e => e.Subdomain).IsUnique();

                entity.Property(t => t.Id)
                    .ValueGeneratedOnAdd();
            });
            #endregion

            #region User Model
            modelBuilder.Entity<User>(entity =>
            {
                // Global query filter for tenant isolation
                entity.HasQueryFilter(u => _tenantService == null || u.TenantId == _tenantService.GetCurrentTenantId());

                entity.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

                // User-Tenant MTO
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Users)
                      .HasForeignKey(u => u.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                // User-UserRole MTO
                entity.HasMany(e => e.UserRoles)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // User-Employee OTO
                entity.HasOne(e => e.Employee)
                      .WithOne(e => e.User)
                      .HasForeignKey<Employee>(e => e.UserId)
                      .IsRequired(false) // User may not have an Employee record
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            });
            #endregion

            #region Employee Model
            modelBuilder.Entity<Employee>(entity =>
            {
                // Global query filter for tenant isolation
                entity.HasQueryFilter(e => _tenantService == null || e.TenantId == _tenantService.GetCurrentTenantId());

                entity.HasIndex(e => new { e.TenantId, e.EmployeeNumber }).IsUnique();

                // Employee-Tenant MTO
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Employees)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete on Tenant deletion
                
                // Employee-User OTO
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Employee)
                      .HasForeignKey<Employee>(e => e.UserId)
                      .IsRequired(true) // Employee must have a User
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Employee-Department MTO
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Employee-Manager OTM (self-referencing)
                entity.HasOne(e => e.Manager)
                      .WithMany(m => m.DirectReports)
                      .HasForeignKey(e => e.ManagerId)
                      .IsRequired(false) // Manager is optional
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            });
            #endregion

            #region Role Model
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => new { r.TenantId, r.Name }).IsUnique();

                // Role-Tenant MTO
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Roles)
                      .HasForeignKey(r => r.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Role-UserRole MTO
                entity.HasMany(e => e.UserRoles)
                      .WithOne(ur => ur.Role)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Permission Model
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();

                // Permission-RolePermission MTO
                entity.HasMany(p => p.RolePermissions)
                      .WithOne(rp => rp.Permission)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Role-Permission Model
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => rp.RolePermissionId);

                entity.HasIndex(rp => new { rp.RoleId, rp.PermissionId });

                // Configure RolePermissionId as auto-increment identity for MySQL
                entity.Property(rp => rp.RolePermissionId)
                      .ValueGeneratedOnAdd()
                      .UseMySqlIdentityColumn();

                entity.HasOne(r => r.Role)
                      .WithMany(rp => rp.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Permission)
                      .WithMany(rp => rp.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region User-Role Model
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                // UserRole-User MTO
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                // UserRole-Role MTO
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Module Model
            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasIndex(e => e.ModuleName).IsUnique();
            });
            #endregion

            #region MenuItem Model
            // MenuItem configuration
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => e.RequiredPermission);

                // Self-referencing relationship for parent-child
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Module
                entity.HasOne(e => e.Module)
                      .WithMany(e => e.MenuItems)
                      .HasForeignKey(e => e.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region MenuItemPermission Model
            modelBuilder.Entity<MenuItemPermission>(entity =>
            {
                entity.HasKey(e => new { e.MenuItemId, e.PermissionId });

                // MenuItem-Permission MTO
                entity.HasOne(e => e.MenuItem)
                      .WithMany(mi => mi.MenuItemPermissions)
                      .HasForeignKey(e => e.MenuItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Permission-MenuItemPermission MTO
                entity.HasOne(e => e.Permission)
                      .WithMany(p => p.MenuItemPermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            // Optional: Configure table names to override convention
            modelBuilder.Entity<Tenant>().ToTable("Tenants");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Permission>().ToTable("Permissions");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermissions");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<Module>().ToTable("Modules");
            modelBuilder.Entity<MenuItem>().ToTable("MenuItems");
            modelBuilder.Entity<MenuItemPermission>().ToTable("MenuItemPermission");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Timesheet>().ToTable("Timesheets");
        }

        public override int SaveChanges()
        {
            SetTenantId();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTenantId();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetTenantId()
        {
            var currentTenantId = _tenantService.GetCurrentTenantId();

            if (currentTenantId <= 0) return;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added &&
                    entry.Entity.GetType().GetProperty("TenantId") != null)
                {
                    entry.Property("TenantId").CurrentValue = currentTenantId;
                }
            }
        }
        public async Task EnsureCoreTenantCreatedAsync()
        {
            // Check if the core tenant exists
            if (!await Tenants.AnyAsync(t => t.Subdomain == "anex-core"))
            {
                // Create the core tenant
                var coreTenant = new Tenant
                {
                    Name = "Core Tenant",
                    Subdomain = "anex-core",
                    IsActive = true,
                    IsSystem = true,
                    CreatedAt = DateTime.UtcNow
                };
                await Tenants.AddAsync(coreTenant);
                await SaveChangesAsync();
            }
        }

        public async Task EnsureSuperAdminCreatedAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var coreTenant = await Tenants.FirstOrDefaultAsync(t => t.Subdomain == "anex-core");

            // Check if SuperAdmin role exists
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                // Create SuperAdmin role if it doesn't exist (should already be seeded, but just in case)
                var superAdminRole = new Role("SuperAdmin")
                {
                    TenantId = coreTenant!.Id,
                    Description = "Super Administrator with full access to all system functions",
                    IsSystemRole = true
                };
                await roleManager.CreateAsync(superAdminRole);
            }

            // Create a default SuperAdmin user if none exists
            if (!userManager.Users.IgnoreQueryFilters().Any(u => u.Email == "superadmin@anex.com"))
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
