using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebIdentity.Models;
using WebIdentity.Repositories;
using WebIdentity.Security.Default;
using WebIdentity.Security.DynamicRole;

namespace WebIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
           
            services.AddDbContextPool<AppDbContext>(optionsAction: options => {
                options.UseSqlServer(Configuration.GetConnectionString(name: "sqlserver"));
            });


            services.AddAuthentication()
                .AddGoogle(options=> {
                    options.ClientId = "904398881580-7l1gvcsm9opp9p8mn20eugrkcb4oetl5.apps.googleusercontent.com";
                    options.ClientSecret = "in-edEyjePgukiSfwbYRSD_i";
                
                });

            services.AddIdentity<IdentityUser, IdentityRole>(options=> {
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 2;
                
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/AccessDenied";
                options.Cookie.Name = "IdentityProj";
                options.LoginPath = "/Login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            services.Configure<SecurityStampValidatorOptions>(option =>
            {
                option.ValidationInterval = TimeSpan.FromMinutes(30);
            });
            services.AddAuthorization(option =>
            {
                //option.AddPolicy("EmployeeListPolicy",
                //    policy => policy
                //        .RequireClaim(ClaimTypesStore.EmployeeList)
                //        .RequireClaim(ClaimTypesStore.EmployeeEdit));


                //option.AddPolicy("ClaimOrRole", policy =>
                //     policy.RequireAssertion(ClaimOrRole));

                //option.AddPolicy("ClaimRequirement", policy =>
                //     policy.Requirements.Add(new ClaimRequirement(ClaimTypesStore.EmployeeList, true.ToString())));

                option.AddPolicy("DynamicRole", policy =>
                  policy.Requirements.Add(new DynamicRoleRequirement()));
            });
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<IMessageSender, MessageSender>();
            services.AddTransient<IUtilities, Utilities>();
            services.AddScoped<IAuthorizationHandler, DynamicRoleHandler>();
            services.AddSingleton<IAuthorizationHandler, ClaimHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private bool ClaimOrRole(AuthorizationHandlerContext context)
            => context.User.HasClaim(ClaimTypesStore.EmployeeList, true.ToString()) ||
               context.User.IsInRole("Admin");
    }
}
