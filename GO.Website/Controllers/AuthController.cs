namespace GO.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly ICryptoService cryptoService;
        private readonly IAuthService authService;
        private readonly IUserService userService;

        public AuthController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            ICryptoService cryptoService,
            IAuthService authService,
            IUserService userService)
        {
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.cryptoService = cryptoService;
            this.authService = authService;
            this.userService = userService;
        }

        public IActionResult Login()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            return View("Login", token);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginParam model)
        {
            model.IP = HttpContext.Connection?.RemoteIpAddress?.ToString();
            model.Platform = HttpContext.Request?.Headers["User-Agent"].ToString();
            model.BrowserName = HttpContext.Request?.Headers["User-Agent"].ToString();
            model.NetSessionId = HttpContext.Session.Id;

            var loginResult = await authService.LoginAsync(model);
            if (!loginResult.IsSuccessfull)
            {
                return Json(new CommandResult { Code = 1, Message = loginResult.ErrorMessage });
            }

            userControlSession.UserId = loginResult.UserId;
            userControlSession.SiteId = userControlSettings.SiteId;
            userControlSession.OnUserId = loginResult.OnUserId;
            userControlSession.JwtToken = loginResult.JwtToken;
            userControlSession.MenuItems = await userService.GetMenuItemAsync();
            userControlSession.Functions = await userService.GetFunctionPermissionAsync(userControlSettings.SiteId, userControlSession.UserId);

            return Json(new CommandResult { Code = 0, Message = string.Empty });

        }

        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync(userControlSession.OnUserId);
            userControlSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}