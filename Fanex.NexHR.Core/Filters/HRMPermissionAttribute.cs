namespace Fanex.NexHR.Core.Filters
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HRMPermissionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public HRMPermissionAttribute(Policy policy, string functionKey)
        {
            Policy = policy;
            FunctionKey = functionKey;
        }

        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var currentSession = new SessionService();

            if (currentSession.Employee.Id <= 0 || !currentSession.HasPermissionOnFunction(FunctionKey, Policy))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }
    }
}