
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using portal_agile.Authorization.Handlers;
using portal_agile.Authorization.Requirements;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Data;
using portal_agile.Helpers;
using portal_agile.Mappings;
using portal_agile.Models;
using portal_agile.Policy.Provider;
using portal_agile.Repositories;
using portal_agile.Security;
using portal_agile.Services;

namespace portal_agile
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            await ConfigureApp(app);

            app.Run();
        }

        // Method to configure services
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext and Identity services
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 4, 3)))
            );

            // Add Identity with custom settings
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 16;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Register Services
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();

            // Register Repositories
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register authorization handlers
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            #region Register Helpers
            services.AddScoped<IInputValidator, InputValidator>();
            #endregion

            // Add permission policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission:Create", policy => policy.Requirements.Add(new PermissionRequirement("Create")));
                options.AddPolicy("Permission:Read", policy => policy.Requirements.Add(new PermissionRequirement("Read")));
                options.AddPolicy("Permission:Update", policy => policy.Requirements.Add(new PermissionRequirement("Update")));
                options.AddPolicy("Permission:Delete", policy => policy.Requirements.Add(new PermissionRequirement("Delete")));
            });

            // Add AutoMapper configuration
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            // Add Controllers
            services.AddControllers();

            // Add Swagger and OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pulse Anex", Version = "v1" });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // Method to configure the HTTP request pipeline
        private async static Task ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pulse Anex API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Middleware configuration
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Initialize and migrate the database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();

                // Ensure SuperAdmin user and role exist
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                await dbContext.EnsureSuperAdminCreatedAsync(userManager, roleManager);
            }
        }
    }
}
