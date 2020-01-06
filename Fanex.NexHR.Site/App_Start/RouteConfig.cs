namespace Fanex.NexHR.Site
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "ManageTeam",
               url: "Employee/AddTeam/{teamId}",
               defaults: new { controller = "employee", action = "AddTeam", teamId = UrlParameter.Optional });

            routes.MapRoute(
               name: "MonthlyWorkingHours",
               url: "Report/MonthlyWorkingHours",
               defaults: new { controller = "MonthlyWorkingHours", action = "index" });

            routes.MapRoute(
               name: "WeeklyWorkingHours",
               url: "Report/WeeklyWorkingHours",
               defaults: new { controller = "WeeklyWorkingHours", action = "index" });

            routes.MapRoute(
               name: "AttendanceReport",
               url: "Report/Attendance",
               defaults: new { controller = "AttendanceReport", action = "index" });

            routes.MapRoute(
               name: "AttendanceDetailsReport",
               url: "Report/AttendanceDetails",
               defaults: new { controller = "AttendanceReport", action = "Details" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional });
        }
    }
}