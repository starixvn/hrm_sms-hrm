namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Company;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly IUserService userService;
        private readonly ICryptoService cryptoService;
        private readonly ISecurityService securityService;

        public AuthController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IBaseService baseService,
            IUserService userService,
            ICryptoService cryptoService,
            ISecurityService securityService)
        {
            this.sessionService = sessionService;
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.securityService = securityService;
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

            LoginResult loginResult = securityService.Login(model);
            if (loginResult.IsSuccessfull)
            {
                userControlSession.UserId = loginResult.UserId;
                userControlSession.JwtToken = loginResult.Token;
                userControlSession.MenuItems = await userService.GetMenuItemAsync();
                userControlSession.Functions = await userService.GetFunctionPermissionAsync();

                await DoLoginAsync(loginResult.UserId);

                return Json(new CommandResult { Code = 0, Message = string.Empty });
            }

            return Json(new CommandResult { Code = 1, Message = loginResult.ErrorMessage });
        }

        public async Task<IActionResult> Logout()
        {
            sessionService.ClearAll();
            userControlSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        protected internal virtual async Task DoLoginAsync(int userId)
        {
            sessionService.Companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}