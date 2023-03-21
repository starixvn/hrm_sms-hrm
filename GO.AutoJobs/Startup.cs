namespace GO.AutoJobs
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using GO.AutoJobs.Middlewares;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.UserSite;
    using GO.AutoJobs.Services;
    using GO.AutoJobs.Workers;
    using HRM.UserControl.Core.Repository;
    using HRM.UserControl.Core.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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

            services.AddSingleton(configuration);
            services.AddHttpClient();

            var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSettings>>();
            services.AddSingleton(jobSettings);

            var nexHrmApiConfig = configuration.GetSection("NexHrmApiConfig").Get<NexHrmApiConfig>();
            services.AddSingleton(nexHrmApiConfig);

            var hrppApiConfig = configuration.GetSection("HrppApiConfig").Get<HrppApiConfig>();
            services.AddSingleton(hrppApiConfig);

            var copyUserToAnotherSiteSettings = configuration.GetSection("CopyUserToAnotherSiteSettings").Get<List<CopyUserToAnotherSiteSetting>>();
            services.AddSingleton(copyUserToAnotherSiteSettings);
        }

        private static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IBaseService, BaseService>();

            services.AddSingleton<INexHrmService, NexHrmService>();
            services.AddSingleton<IHrppUnitHeadService, HrppUnitHeadService>();
            services.AddSingleton<IUserSiteService, UserSiteService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<NexHrmEmployeeInfoWorker>();
            services.AddHostedService<NexHrmTeamWorker>();
            services.AddHostedService<NexHrmUnitHeadInfoWorker>();
            services.AddHostedService<NexHrmUnitHeadScheduleWorker>();

            services.AddHostedService<LumiHrmEmployeeInfoWorker>();
            services.AddHostedService<LumiHrmTeamWorker>();

            services.AddHostedService<HrppUnitHeadInfoWorker>();
            services.AddHostedService<HrppUnitHeadScheduleWorker>();

            services.AddHostedService<CopyUserToAnotherSiteWorker>();
        }
    }
}