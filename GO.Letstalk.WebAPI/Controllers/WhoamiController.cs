namespace GO.Letstalk.WebAPI.Controllers
{
    using GO.Letstalk.WebAPI.Extentions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class WhoamiController : Controller
    {
        [Route("Whoami")]
        [Route("Whoami.aspx")]
        public IActionResult Whoami()
        {
            return Content(WhoamiHelper.GetContent(HttpContext));
        }
    }
}