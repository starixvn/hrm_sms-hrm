namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}