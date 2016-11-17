namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Nexus.RoleManagement.Services;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HRAuthorizeAttribute : AuthorizeAttribute
    {
        public HRAuthorizeAttribute(Policy policy, params string[] functionKey)
        {
            Policy = policy;
            FunctionKey = functionKey;
        }

        public Policy Policy { get; set; }

        public string[] FunctionKey { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var result = AuthorizeCore(filterContext.HttpContext);
            if (!result)
            {
                filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (isAuthorized)
            {
                var user = (UserLoginModel)httpContext.Session["CurrentUser"];
                foreach (var functionKey in FunctionKey)
                {
                    var hasPermission = user.HasPermission(functionKey, Policy);
                    if (hasPermission)
                    {
                        return hasPermission;
                    }
                }
            }

            return false;
        }
    }
}
