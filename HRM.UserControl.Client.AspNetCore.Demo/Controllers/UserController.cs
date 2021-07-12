using System.Threading.Tasks;
using HRM.UserControl.Client.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.UserControl.Client.AspNetCore.Demo.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userService.GetByEmailAsync("oliver.dang@nexcel.vn");
            return Ok(user);
        }
    }
}