using System;
using System.Net;
using System.Web.Configuration;
using Fanex.Common;
using Fanex.NexHR.Core;
using Fanex.NexHR.Services.Extensions;
using Fanex.NexHR.Site.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Unity;

[assembly: OwinStartup(typeof(Fanex.NexHR.Site.Startup))]

namespace Fanex.NexHR.Site
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            var connectionString = Cryptography.DeCryptConnectionString(WebConfigurationManager.ConnectionStrings["nexhrx"].ConnectionString);
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            var unityContainer = new UnityContainer();
            GlobalConfiguration.Configuration.UseActivator(new UnityJobActivator(unityContainer));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();

            HangfireJobScheduler.ScheduleRecurringJobs();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
        }
    }

    public class UnityJobActivator : JobActivator
    {
        private readonly IUnityContainer unityContainer;

        public UnityJobActivator(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.unityContainer.ResolveCoreDI();
            this.unityContainer.ResolveServiceCoreDI();
        }

        public override object ActivateJob(Type jobType)
        {
            return unityContainer.Resolve(jobType);
        }
    }
}