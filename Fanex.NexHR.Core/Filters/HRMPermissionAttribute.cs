namespace Fanex.NexHR.Core.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HRMPermissionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public HRMPermissionAttribute(Policy policy, params string[] functionKeys)
        {
            Policy = policy;
            FunctionKeys = functionKeys;
        }

        public string[] FunctionKeys { get; set; }

        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var userControlSession = new UserControlSession();
            if (userControlSession.UserId <= 0
                || !FunctionKeys.Select(functionKey => userControlSession.HasPermissionOnFunction(functionKey, Policy)).Any(x => x))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }
    }
}