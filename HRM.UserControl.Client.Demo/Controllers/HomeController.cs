namespace HRM.UserControl.Client.Demo.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Services.Interfaces;
    using HRM.UserControl.Client.Core.Session;

    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly ISecurityService securityService;
        private readonly IUCSession ucSession;

        public HomeController() : this(null, null, null)
        {
        }

        public HomeController(IUserService userService, IUCSession ucSession, ISecurityService securityService)
        {
            this.userService = userService ?? new UserService();
            this.ucSession = ucSession ?? new UCSession();
            this.securityService = securityService ?? new SecurityService();
        }

        public async Task<ActionResult> Index()
        {
            var user = await userService.GetByUsernameAsync("oliver.dang");
            if (user != null)
            {
                var loginResult = await LoginAsync(user);

                if (loginResult.IsLoginSuccess)
                {
                    ucSession.UserId = loginResult.UserId;
                    ucSession.OnCompanyId = loginResult.CompanyId;
                    ucSession.JwtToken = loginResult.Token;
                    ucSession.Roles = await GetUserRolesOnCompany();
                }
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReCheckIn()
        {
            var newToken = await securityService.RefreshTokenAsync();
            ucSession.JwtToken = newToken;
            return Json(ucSession);
        }

        private async Task<IList<CompanyRoleFunction>> GetUserRolesOnCompany()
        {
            var roles = await userService.GetPermissionAsync();

            return roles;
        }

        private async Task<LoginResult> LoginAsync(User user)
        {
            var loginParam = new LoginParam
            {
                UserId = user.UserId,
                CompanyId = 2,
                Ip = Request.UserHostAddress,
                Platform = Request.Headers["User-Agent"],
                BrowserName = Request.Headers["User-Agent"]
            };
            var loginResult = await securityService.LoginAsync(loginParam);

            return loginResult;
        }
    }
}