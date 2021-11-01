namespace HRM.UserControl.Client.AspNetCore.Demo.Controllers
{
    using HRM.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return Ok();
        }
    }
}