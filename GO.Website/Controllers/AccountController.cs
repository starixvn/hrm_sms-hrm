namespace GO.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        public IActionResult Setup2FA()
        {
            return View();
        }
    }
}