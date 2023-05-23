namespace GO.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        [Route("error/404")]
        [Route("error/PageNotFound")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("error/405")]
        [Route("error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("error/500")]
        [Route("error/InternalError")]
        public IActionResult InternalError()
        {
            return View();
        }
    }
}