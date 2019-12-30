namespace Fanex.NexHR.Site
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.NexHR.Core.Providers;
    using Fanex.NexHR.Resources;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer(
                    resourcePath: "~/"
                )
                .Run();
            ModelMetadataProviders.Current = new FriendlyNameModelMetadataProvider();
        }

        [ValidateInput(false)]
        protected void Application_Error(object sender, EventArgs e)
        {
            if (!Request.IsLocal)
            {
                Exception ex = Server.GetLastError().GetBaseException();
                string message = string.Format(
                    "{0}{1}{2}{3}{1}{4}{5}{1}{6}{7}{1}{8}{9}",
                    ex.Message,
                    Environment.NewLine,
                    Labels.Source,
                    ex.Source,
                    Labels.Domain,
                    HttpContext.Current.Request.Headers["Host"],
                    Labels.TargetSite,
                    ex.TargetSite,
                    Labels.StackTrace,
                    ex.StackTrace);

                LogService.Error(message);
            }
        }
    }
}