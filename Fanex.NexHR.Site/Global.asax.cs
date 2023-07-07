namespace Fanex.NexHR.Site
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NexHR.Core.Providers;
    using HRM.UserControl.Client.AspNetMvc.Configurations;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core;
    using SharpRaven;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            LogManager
                .SetDefaultLogCategory(ConfigurationManager.AppSettings["Sentry:Category"])
                .Use(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry:Dsn"]),
                    WriteLogTimeoutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["Sentry:Timeout"])
                }));

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer("~/App_Data/SpNames.xml")
                .Run();
            ModelMetadataProviders.Current = new FriendlyNameModelMetadataProvider();

            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            UserControlClientManager
               .UseSettings(new UserControlSettings())
               .UseSession(new UserControlSession());
        }

        [ValidateInput(false)]
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            ExceptionHandler.Handle(ex, TraceEventType.Error);
        }
    }
}