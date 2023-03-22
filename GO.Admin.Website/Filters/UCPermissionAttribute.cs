namespace GO.Admin.Website.Filters
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UCPermissionAttribute : Attribute, IAsyncAuthorizationFilter
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

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
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
                if (!session.HasPermissionOnFunction(FunctionKey, Policy))
                {
                    if (IsAjaxRequest(context.HttpContext.Request))
                    {
                        var response = context.HttpContext.Response;
                        response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        await response.WriteAsync("<center><b>Access denied!</b></center>");
                    }
                    else
                    {
                        context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                    }
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}