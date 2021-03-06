using System;
using System.Text;
using UserManagement_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement_Backend.Context;
using Microsoft.Extensions.Configuration;
using UserManagement_Backend.Services.Auths;
using UserManagement_Backend.Services.Roles;
using UserManagement_Backend.Services.Users;
using UserManagement_Backend.Services.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserManagement_Backend.Services.UserRoles;
using UserManagement_Backend.Services.Emails;

namespace UserManagement_Backend.Helpers
{
    public static class ServiceExtensions
    {
        // Configure for CORS
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });
        }

        // Configure for Logger
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }

        // Configure for User Service
        public static void ConfigureUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }

        // Configure for Auth Service
        public static void ConfigureAuthService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }

        // Configure for Role Service
        public static void ConfigureRoleService(this IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
        }

        // Configure for User Roles Service
        public static void ConfigureUserRolesService(this IServiceCollection services)
        {
            services.AddScoped<IUserRoleService, UserRoleService>();
        }

        // Configure for Permission Service
        public static void ConfigurePolicyService(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Policy for Admin
                options.AddPolicy(Authorization.ADMIN_ONLY, policy =>
                {
                    policy.RequireRole(
                        Authorization.Roles.Administrator.ToString()
                    );
                });

                // Policy for Manager
                options.AddPolicy(Authorization.MANAGER_ONLY, policy =>
                {
                    policy.RequireRole(
                        Authorization.Roles.Administrator.ToString(),
                        Authorization.Roles.Moderator.ToString()
                    );
                });

                // Policy for Basic User
                options.AddPolicy(Authorization.VIEW_ONLY, policy =>
                {
                    policy.RequireRole(
                        Authorization.Roles.User.ToString()
                    );
                });
            });
        }

        // Configure for JWT
        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<JWT>(Configuration.GetSection("JWT"));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = false;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                };
            });
        }

        // Configure for DatabaseContext
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(configuration.GetConnectionString("UserManagementDbConnection"),
                    MySqlServerVersion.LatestSupportedServerVersion);
            });
        }

        // Configure for Email Sending Service
        public static void ConfigureEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
