// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Anders.Le" date="2012.8.27">Add application error</change>
// </history>
namespace Fanex.NexHR.Site
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Data.Configuration;
    using Fanex.NexHR.Web.Providers;

    /// <summary>
    /// MVC Application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Register Routes.
        /// </summary>
        /// <param name="routes">Route Collection.</param>
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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DbSettingProviderManager.Current.Start();
            ModelMetadataProviders.Current = new FriendlyNameModelMetadataProvider();
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [ValidateInput(false)]
        protected void Application_Error(object sender, EventArgs e)
        {
            ////var exception = Server.GetLastError().GetBaseException();
            ////LogService.Error(exception.Message, exception);

            ////var serverError = Server.GetLastError() as HttpException;
            ////int errorCode = serverError != null ? serverError.GetHttpCode() : (int)HttpStatusCode.InternalServerError;

            ////if (!Response.IsRequestBeingRedirected)
            ////{
            ////    Response.Clear();
            ////    var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            ////    Response.Redirect(string.Format("/Error/Index?errorCode={0}", errorCode));
            ////    Server.ClearError();
            ////}
        }
    }
}