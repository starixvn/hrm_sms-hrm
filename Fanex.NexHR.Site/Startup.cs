using Fanex.NexHR.Site.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Fanex.NexHR.Site.Startup))]
namespace Fanex.NexHR.Site
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();
        }
    }
}