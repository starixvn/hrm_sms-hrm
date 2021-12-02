namespace HRM.UserControl.Admin.Website.Filters
{
    using System;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UCPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IUserControlSession session;

        public UCPermissionAttribute() : this(null)
        {
        }

        internal UCPermissionAttribute(IUserControlSession session)
        {
            this.session = session ?? UserControlClientManager.UserControlSession;
        }

        public string FunctionKey { get; set; }
        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (session.UserId <= 0)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    context.Result = new RedirectToActionResult("logout", "auth", null);
                }
            }
            else
            {
                if (!session.HasPermissionOnFunction(FunctionKey, Policy))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
    }
}