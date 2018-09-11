namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Models;

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
    }
}