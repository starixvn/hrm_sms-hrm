namespace HRM.UserControl.Admin.Website.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}