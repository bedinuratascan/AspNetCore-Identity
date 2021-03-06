using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.CustomValidation;
using membershipSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace membershipSystem
{
    public class Startup
    {
        public IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString:DefaultConnectionString"]);
            });

            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AnkaraPolicy", pol =>
                {
                    pol.RequireClaim("City", "Ankara");
                });
                opt.AddPolicy("ViolencePolicy", pol =>
                {
                    pol.RequireClaim("Violence");
                });
                opt.AddPolicy("ExchangePolicy", pol =>
                {
                    pol.AddRequirements(new ExpireDateExchangeRequirement());
                });
            });

            services.AddAuthentication().AddFacebook(opt =>
            {
                opt.AppId = Configuration["Authentication:Facebook:AppId"];
                opt.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(opt =>
            {
                opt.ClientId= Configuration["Authentication:Google:ClientID"];
                opt.ClientSecret= Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddIdentity<AppUser, AppRole>(option=> 
            {
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters = "abc�def�gh�ijklmno�pqrs�tu�vwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                option.Password.RequiredLength = 4;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireDigit = false;
            }).AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidator>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "blog";
            cookieBuilder.HttpOnly = false;
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Login");
                options.LogoutPath = new PathString("/Member/LogOut");
                options.Cookie = cookieBuilder;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.AccessDeniedPath = new PathString("/Member/AccessDenied");
            });

            services.AddScoped<IClaimsTransformation, ClaimProvider.ClaimProvider>();

            services.AddMvc(option => option.EnableEndpointRouting = false);

         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
