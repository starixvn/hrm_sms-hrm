namespace Fanex.NexHR.Site
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            routes.MapRoute(
               name: "ManageTeam",
               url: "Employee/AddTeam/{teamId}",
               defaults: new { controller = "employee", action = "AddTeam", teamId = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "employee", action = "myspace", id = UrlParameter.Optional });
        }
    }
}