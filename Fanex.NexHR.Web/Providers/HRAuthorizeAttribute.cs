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
        public Policy Policy { get; set; }

        public string FunctionKey { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            /* var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            var user = (UserLoginModel)httpContext.Session["CurrentUser"];
            var function = user.NE_Functions.Where(i => i.FunctionKey.Equals(FunctionKey));
            if (function != null)
            {
                return Policy == Policy.Read ? function.First().IsRead || function.First().IsDelete : function.First().IsDelete;
            }
            */

            return true;
        }
    }
}
