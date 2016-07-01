namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Employee = CurrentUser;
            return RedirectToAction("myspace", "employee");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult AccountSuspended()
        {
            return View();
        }
    }
}
