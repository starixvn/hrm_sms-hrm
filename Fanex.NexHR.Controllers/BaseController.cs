namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Models;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    [Authorize]
    public class BaseController : Controller
    {
        public UserLoginModel CurrentUser
        {
            get
            {
                return (UserLoginModel)HttpContext.Session["CurrentUser"];
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (HttpContext.Session["CurrentUser"] == null ||
                    !HttpContext.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();

                if (!Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
                else
                {
                    string lostLogin = "Session is expired. Please press F5 to login again.";
                    var contentType = filterContext.HttpContext.Request.ContentType ?? "text/html";
                    if (contentType.Contains("json"))
                    {
                        var result = new JsonResult();
                        result.Data = new { Code = -101, Message = lostLogin };
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = result;
                    }
                    else
                    {
                        filterContext.Result = Content(lostLogin);
                    }
                }
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            UserLoginModel currentUser = this.CurrentUser;

            var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            if (attributes.Length == 0)
            {
                attributes = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
            }

            if (attributes.Length == 0)
            {
                return;
            }

            if (currentUser != null)
            {
                foreach (AuthorizeAttribute item in attributes)
                {
                    if (!string.IsNullOrWhiteSpace(item.Roles) && !currentUser.IsInRoles(item.Roles.Split(new[] { ',' }).ToList()))
                    {
                        filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url.AbsoluteUri);
                    }
                }
            }
        }
    }
}
