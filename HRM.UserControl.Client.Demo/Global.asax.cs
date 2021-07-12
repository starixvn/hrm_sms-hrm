using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HRM.UserControl.Client.AspNetMvc.Configurations;
using HRM.UserControl.Client.AspNetMvc.Session;
using HRM.UserControl.Client.Core;

namespace HRM.UserControl.Client.Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UserControlClientManager
                .UseSettings(new UserControlSettings())
                .UseSession(new UCSession());
        }
    }
}