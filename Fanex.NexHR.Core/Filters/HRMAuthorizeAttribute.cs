namespace Fanex.NexHR.Core.Filters
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Core.Services;
    using HRM.UserControl.Client.Core;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HRMAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false)
                && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                var currentSession = new SessionService();
                if (currentSession.Employee == null || currentSession.Employee.Id <= 0 || !IsValidUserControlSession())
                {
                    FormsAuthentication.SignOut();

                    if (!filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                    }
                    else
                    {
                        var response = filterContext.HttpContext.Response;
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Write("<center><b>Unauthorized</b></center>");
                    }
                }
            }
        }

        private static bool IsValidUserControlSession()
        {
            var ucsession = UserControlClientManager.UserControlSession;
            return !(ucsession.UserId <= 0 || ucsession.JwtToken == null || ucsession.JwtToken.IsExpired);
        }
    }
}