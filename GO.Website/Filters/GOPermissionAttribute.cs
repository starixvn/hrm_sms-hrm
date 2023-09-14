namespace GO.Website.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class GOPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IUserControlSession session;

        public GOPermissionAttribute() : this(null)
        {
        }

        internal GOPermissionAttribute(IUserControlSession session)
        {
            this.session = session ?? UserControlClientManager.UserControlSession;
        }

        public string FunctionKey { get; set; }
        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (session.UserId <= 0 || session.JwtToken == null || session.JwtToken.IsExpired)
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
                CheckUserPermission(context);
            }
        }

        private void CheckUserPermission(AuthorizationFilterContext context)
        {
            var attributes = context.ActionDescriptor.FilterDescriptors.Where(x => x.Filter is GOPermissionAttribute)
                    .Select(x => x.Filter as GOPermissionAttribute).ToList();
            bool hasPermission = false;

            foreach (var attribute in attributes)
            {
                if (session.HasPermissionOnFunction(attribute.FunctionKey, attribute.Policy))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
                }
                else
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}