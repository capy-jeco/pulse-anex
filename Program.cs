
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using portal_agile.Authorization.Handlers;
using portal_agile.Authorization.Requirements;
using portal_agile.Contracts.Data;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Data;
using portal_agile.Data.Seeders;
using portal_agile.Helpers;
using portal_agile.Mappings;
using portal_agile.Middlewares;
using portal_agile.Models;
using portal_agile.Policy.Provider;
using portal_agile.Repositories;
using portal_agile.Security;
using portal_agile.Services;
using System.Reflection;
using System.Text;

namespace portal_agile
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            await ConfigureApp(app);

            app.Run();
        }

        // Method to configure services
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            #region Database Conext Configuration
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 4, 3)))
            );
            #endregion

            #region CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnexCore", policy =>
                {
                    policy.WithOrigins("http://anex-core.pulse.anex.com:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Needed if you're sending cookies or auth headers
                });
            });
            #endregion

            #region API Versioning Configuration
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("version"),
                    new HeaderApiVersionReader("X-Version")
                );
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            #endregion

            #region HTTP Context Accessor Service Injection
            services.AddHttpContextAccessor();
            #endregion

            #region Login Identity Configuration
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
            #endregion

            #region Services Registration
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMenuService, MenuService>();
            #endregion

            #region Repositories Registration
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion

            #region Authorization Handlers Registration
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            #endregion

            #region Register Helpers
            services.AddScoped<IInputValidator, InputValidator>();
            #endregion

            #region Database Seeders Registration
            services.AddTransient<ISeeder, SystemRoleSeeder>();
            services.AddTransient<ISeeder, SystemPermissionSeeder>();
            services.AddTransient<ISeeder, SystemRolePermissionSeeder>();
            services.AddTransient<ISeeder, SystemSuperAdminSeeder>();
            services.AddTransient<ISeeder, ModuleSeeder>();
            services.AddTransient<ISeeder, MenuItemSeeder>();
            services.AddTransient<ISeeder, MenuItemPermissionSeeder>();

            services.AddTransient<SeederManager>();
            #endregion

            #region Authentication and Authorization Configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
            #endregion

            #region Permisison Policies Configuration
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission:Create", policy => policy.Requirements.Add(new PermissionRequirement("Create")));
                options.AddPolicy("Permission:Read", policy => policy.Requirements.Add(new PermissionRequirement("Read")));
                options.AddPolicy("Permission:Update", policy => policy.Requirements.Add(new PermissionRequirement("Update")));
                options.AddPolicy("Permission:Delete", policy => policy.Requirements.Add(new PermissionRequirement("Delete")));
            });
            #endregion

            #region Automapper Configuration
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region Controllers Configuration
            services.AddControllers();
            #endregion

            #region Swagger Configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pulse Anex", Version = "v1" });

                c.AddServer(new OpenApiServer
                {
                    //Url = $"{(environment.IsDevelopment() ? "http" : "https")}://pulse.anex.com:8000"
                    Url = "http://pulse.anex.com:8000"
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion
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
            //app.UseHttpsRedirection(); // -> Use this on production
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("AllowAnexCore");

            app.UseMiddleware<TenantMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Initialize and migrate the database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();

                await dbContext.EnsureCoreTenantCreatedAsync();

                var seederManager = scope.ServiceProvider.GetRequiredService<SeederManager>();
                await seederManager.SeedAsync();
            }
        }
    }
}
