using Fanex.NexHR.Models;
using Nexus.RoleManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Fanex.NexHR.Web.Providers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HRAuthorizeAttribute : AuthorizeAttribute
    {
        public HRAuthorizeAttribute(Policy policy, params string[] functionKey)
        {
            Policy = policy;
            FunctionKey = string.Join(",", functionKey);
        }

        private Policy Policy { get; set; }

        private string FunctionKey { get; set; }

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
                foreach (var functionKey in FunctionKey.Split(','))
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
