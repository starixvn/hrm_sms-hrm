namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;

    public class HomeController : BaseController
    {
        public HomeController(ISessionService sessionService) : base(sessionService)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("myspace", "employee");
        }
    }
}