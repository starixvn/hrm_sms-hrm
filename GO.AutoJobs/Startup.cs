namespace GO.AutoJobs
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using GO.AutoJobs.Middlewares;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.UserSite;
    using GO.AutoJobs.Services;
    using GO.AutoJobs.Workers;
    using GO.Core.Models;
    using GO.Core.Repository;
    using GO.Core.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sentry;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddConfiguration(services, Configuration);
            AddDependencyInjection(services);
            AddWorker(services);
            services.AddControllers();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlerMiddleware>();
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var connections = configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                .ToDictionary(x => x.GetValue<string>("Name"),
                y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithSqlServer()
                .UseDefaultDbSettingProvider("~/App_Data/SpNames.xml")
                .Run();

            LogManager
                .SetDefaultLogCategory(configuration.GetSection("Sentry:LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(configuration.GetSection("Sentry:Dsn").Value)
                    })
                });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddSingleton(configuration);
            services.AddHttpClient();

            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSettings>>();
            services.AddSingleton(jobSettings);

            var nexHrmApiConfig = configuration.GetSection("NexHrmApiConfig").Get<NexHrmApiConfig>();
            services.AddSingleton(nexHrmApiConfig);

            var hrppApiConfig = configuration.GetSection("HrppApiConfig").Get<HrppApiConfig>();
            services.AddSingleton(hrppApiConfig);

            var copyUserToAnotherSiteSettings = configuration.GetSection("CopyUserToAnotherSiteSettings").Get<List<CopyUserToAnotherSiteSetting>>();
            services.AddSingleton(copyUserToAnotherSiteSettings);

            var mailConfigs = configuration.GetSection("MailConfigs").Get<MailConfigs>();
            services.AddSingleton(mailConfigs);
        }

        private static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseMailService, BaseMailService>();

            services.AddSingleton<INexHrmService, NexHrmService>();
            services.AddSingleton<ISyncDataFromDBService, SyncDataFromDBService>();
            services.AddSingleton<IHrppUnitHeadService, HrppUnitHeadService>();
            services.AddSingleton<IUserSiteService, UserSiteService>();
            services.AddSingleton<IManpowerForecastService, ManpowerForecastService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<NexHrmWorker>();
            services.AddHostedService<NexHrmUnitHeadInfoWorker>();
            services.AddHostedService<NexHrmUnitHeadScheduleWorker>();

            services.AddHostedService<LumiHrmWorker>();
            services.AddHostedService<SmsHrmWorker>();
            services.AddHostedService<HongMaHrmWorker>();
            services.AddHostedService<CtiHrmWorker>();
            services.AddHostedService<LexfinHrmWorker>();

            services.AddHostedService<HrppUnitHeadInfoWorker>();
            services.AddHostedService<HrppUnitHeadScheduleWorker>();
            services.AddHostedService<ManpowerForecastWorker>();

            services.AddHostedService<CopyUserToAnotherSiteWorker>();
        }
    }
}