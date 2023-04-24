namespace GO.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using GO.Website.Hubs;
    using GO.Website.Middlewares;
    using GO.Website.Models;
    using GO.Website.Services;
    using HRM.UserControl.Client.AspNetCore.Configurations;
    using HRM.UserControl.Client.AspNetCore.Session;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Connections;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;

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
            var connections = Configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                .ToDictionary(x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithSqlServer()
               .UseDefaultDbSettingProvider("~/App_Data")
               .Run();

            services.AddHttpClient();
            services.AddResponseCaching();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = Configuration.GetSection("AuthSettings:LoginPath").Value;
                    options.LogoutPath = Configuration.GetSection("AuthSettings:LogoutPath").Value;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:SessionTimeout").Value));
                    options.SlidingExpiration = true;
                    options.Cookie = new CookieBuilder
                    {
                        Name = ".GO.System.Cookies",
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    options.Events.OnSigningOut = context =>
                    {
                        context.Response.Redirect(options.LoginPath);
                        return Task.CompletedTask;
                    };
                }).AddJwtBearer(x =>
                {
                    byte[] secretKey = Encoding.ASCII.GetBytes(Configuration.GetSection("JwtAuthSettings:SecretKey").Value);
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".GO.System.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:SessionTimeout").Value));
                options.Cookie.IsEssential = true;
            });

            var jwtAuthSettings = Configuration.GetSection("JwtAuthSettings").Get<JwtAuthSettings>();
            services.AddSingleton(jwtAuthSettings);

            var apiSiteSettings = Configuration.GetSection("ApiSiteSettings").Get<List<ApiSiteSetting>>();
            services.AddSingleton(apiSiteSettings);

            RegisterDI(services);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UserControlClientManager
                   .UseSettings(app.ApplicationServices.GetService<IUserControlSettings>())
                   .UseSession(app.ApplicationServices.GetService<IUserControlSession>());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlerMiddleware>();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TodolistHub>("/todolist/hubs", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterDI(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserControlSettings>(new UserControlSettings(Configuration));
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IUserControlSession, UserControlSession>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IFunctionService, FunctionService>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IUnitHeadService, UnitHeadService>();
            services.AddSingleton<IEpaTodolistService, EpaTodolistService>();
        }
    }
}