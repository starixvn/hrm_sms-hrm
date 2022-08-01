namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Criteria.Company;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly UserControlConfigs userControlConfigs;
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly IUserService userService;
        private readonly ICryptoService cryptoService;
        private readonly IAuthService authService;

        public AuthController(
            UserControlConfigs userControlConfigs,
            IUserControlSession userControlSession,
            IBaseService baseService,
            IUserService userService,
            ICryptoService cryptoService,
            IAuthService authService)
        {
            this.userControlConfigs = userControlConfigs;
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.authService = authService;
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

            LoginResult loginResult = await authService.LoginAsync(model);
            if (loginResult.IsSuccessfull)
            {
                userControlSession.UserId = loginResult.UserId;
                userControlSession.SiteId = userControlConfigs.SiteId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.MenuItems = await userService.GetMenuItemAsync();
                userControlSession.Functions = await userService.GetFunctionPermissionAsync(userControlConfigs.SiteId, userControlSession.UserId);

                await DoLoginAsync(loginResult.UserId);

                return Json(new CommandResult { Code = 0, Message = string.Empty });
            }

            return Json(new CommandResult { Code = 1, Message = loginResult.ErrorMessage });
        }

        public IActionResult GetHeader()
        {
            var menus = GetMenuTrees(userControlSession.MenuItems);
            var header = new
            {
                menus = menus,
                nickname = string.Empty
            };

            return Json(header);
        }

        private IList<MenuItem> GetMenuTrees(IList<MenuItem> menus)
        {
            IList<MenuItem> menuTrees = menus.Where(menu => !menu.ParentId.HasValue).ToList();
            foreach (MenuItem menu in menuTrees)
            {
                menu.Childs = new List<MenuItem>();
                GetChildMenus(menu, menus);
            }
            return menuTrees;
        }

        private void GetChildMenus(MenuItem menu, IList<MenuItem> menus)
        {
            IList<MenuItem> menuItems = menus.Where(x => x.ParentId == menu.MenuItemId).ToList();
            if (menuItems.Any())
            {
                foreach (MenuItem item in menuItems)
                {
                    menu.Childs.Add(item);
                    GetChildMenus(item, menus);
                }
            }
        }

        public async Task<IActionResult> Logout()
        {
            userControlSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        protected internal virtual async Task DoLoginAsync(int userId)
        {
            var companies = (await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true })).ToList();
            userControlSession.Set<IList<Company>>(SessionKey.InternalCompanies, companies);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}