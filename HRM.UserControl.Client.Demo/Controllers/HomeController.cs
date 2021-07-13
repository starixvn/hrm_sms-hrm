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

    [SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly IUCSession ucSession;

        public HomeController() : this(null, null)
        {
        }

        public HomeController(IUserService userService, IUCSession ucSession)
        {
            this.userService = userService ?? new UserService();
            this.ucSession = ucSession ?? new UCSession();
        }

        public async Task<ActionResult> Index()
        {
            var s = Session;
            var user = await userService.GetByEmailAsync("oliver.dang@nexcel.vn");
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
            var loginResult = await userService.LoginAsync(loginParam);

            return loginResult;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}