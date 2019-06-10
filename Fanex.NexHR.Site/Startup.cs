using System;
using System.Web.Configuration;
using Fanex.Common;
using Fanex.Data.Repository;
using Fanex.NexHR.Core.Repositories;
using Fanex.NexHR.Core.Services;
using Fanex.NexHR.Services;
using Fanex.NexHR.Site.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Unity;
using Unity.Lifetime;

[assembly: OwinStartup(typeof(Fanex.NexHR.Site.Startup))]

namespace Fanex.NexHR.Site
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            var connectionString = Cryptography.DeCryptConnectionString(WebConfigurationManager.ConnectionStrings["nexhrx"].ConnectionString);
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            var hangfireContainer = new UnityContainer();
            GlobalConfiguration.Configuration.UseActivator(new UnityJobActivator(hangfireContainer));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();

            HangfireJobScheduler.ScheduleRecurringJobs();
        }
    }

    public class UnityJobActivator : JobActivator
    {
        private readonly IUnityContainer hangfireContainer;

        public UnityJobActivator(IUnityContainer hangfireContainer)
        {
            this.hangfireContainer = hangfireContainer;
            this.hangfireContainer.RegisterType<IDynamicRepository, DynamicRepository>(new PerResolveLifetimeManager());
            this.hangfireContainer.RegisterType<IBaseRepository, BaseRepository>(new PerResolveLifetimeManager());
            this.hangfireContainer.RegisterType<IBaseService, BaseService>(new PerResolveLifetimeManager());
            this.hangfireContainer.RegisterType<IBackgroundJobService, BackgroundJobService>(new PerResolveLifetimeManager());
        }

        public override object ActivateJob(Type type)
        {
            return hangfireContainer.Resolve(type);
        }
    }
}