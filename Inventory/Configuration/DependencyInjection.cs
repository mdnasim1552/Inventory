using Inventory.Data;
using Inventory.Extensions;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.Repositories;
using Inventory.UnitOfWork;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Options;

namespace Inventory.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllRepository(this IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, Inventory.UnitOfWork.UnitOfWork>();
            services.AddScoped<IProcessAccess, ProcessAccess>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<TwilioService>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IRoleService, RoleService>();
            //services.AddScoped<ILoginRepository, LoginRepository>();
            //services.AddScoped<IPrintRepository, PrintRepository>();
            //services.AddScoped<IMasterpageRepository, MasterpageRepository>();
            return services;
        }
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //options.Cookie.Name = "MyCookieAuth";
                options.LoginPath = "/Account/Signin";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            //services.AddAuthorization(option =>
            //{
            //    option.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
            //    option.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRole.Admin.ToString()));
            //    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole(UserRole.Manager.ToString()));
            //    options.AddPolicy("UserPolicy", policy => policy.RequireRole(UserRole.User.ToString()));
            //    options.AddPolicy("GuestPolicy", policy => policy.RequireRole(UserRole.Guest.ToString()));
            //    options.AddPolicy("ModeratorPolicy", policy => policy.RequireRole(UserRole.Moderator.ToString()));
            //    options.AddPolicy("AuditorPolicy", policy => policy.RequireRole(UserRole.Auditor.ToString()));
            //    options.AddPolicy("SupportPolicy", policy => policy.RequireRole(UserRole.Support.ToString()));
            //});

            services.AddAuthorization(options =>
            {
                // Retrieve roles dynamically from the database
                var _unitOfWork = services.BuildServiceProvider().GetService<IUnitOfWork>();
                var roles = _unitOfWork.Userrole.Find(u => u.Role != Policies.Admin);//_unitOfWork.Userrole.GetAll();
                options.AddPolicy(Policies.Admin, policy => policy.RequireRole(Policies.Admin));
                // Configure policies dynamically
                foreach (var r in roles)
                {
                    options.AddPolicy(r.Role.Replace(" ", ""), policy =>
                        policy.RequireAssertion(context =>
                        context.User.IsInRole(Policies.Admin) || 
                        context.User.IsInRole(r.Role)));
                    //options.AddPolicy($"{r.Role.Replace(" ","")}Policy", policy => policy.RequireRole(Policies.Admin).RequireRole(r.Role));
                    //options.AddPolicy($"{r.Role.Replace(" ", "")}Policy", policy => policy.RequireClaim(r.Role));
                }
            });

            return services;
        }
    }
}
