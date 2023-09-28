namespace GO.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using GO.Core.Models;
    using GO.Core.Repository;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Middlewares;
    using GO.Website.Models;
    using GO.Website.Repositories;
    using GO.Website.Services;
    using GO.Website.Services.Interfaces;
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Sentry;

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

            LogManager
                .SetDefaultLogCategory(Configuration.GetSection("Sentry:LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(Configuration.GetSection("Sentry:Dsn").Value)
                    })
                });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddHttpClient(HttpClientName.DefaultHttpClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                });
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

            RegisterDI(services);

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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterDI(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var jwtAuthSettings = Configuration.GetSection("JwtAuthSettings").Get<JwtAuthSettings>();
            services.AddSingleton(jwtAuthSettings);

            var apiSiteSettings = Configuration.GetSection("ApiSiteSettings").Get<List<ApiSiteSetting>>();
            services.AddSingleton(apiSiteSettings);

            var mailConfigs = Configuration.GetSection("MailConfigs").Get<MailConfigs>();
            services.AddSingleton(mailConfigs);

            var otpConfig = Configuration.GetSection("OtpConfig").Get<OtpConfig>();
            services.AddSingleton(otpConfig);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserControlSettings>(new UserControlSettings(Configuration));
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IUserControlSession, UserControlSession>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<HRM.UserControl.Client.Core.Services.ICryptoService, HRM.UserControl.Client.Core.Services.CryptoService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IFunctionService, FunctionService>();

            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IManpowerForecastRepository, ManpowerForecastRepository>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseMailService, BaseMailService>();
            services.AddSingleton<IExportExcelService, ExportExcelService>();
            services.AddSingleton<IUnitHeadService, UnitHeadService>();
            services.AddSingleton<ITodolistService, TodolistService>();
            services.AddSingleton<IFeedbackService, FeedbackService>();
            services.AddSingleton<IManpowerForecastService, ManpowerForecastService>();
            services.AddSingleton<IUserOtpService, UserOtpService>();
            services.AddSingleton<ITwoFactorAuthenticatorService, TwoFactorAuthenticatorService>();
        }
    }
}