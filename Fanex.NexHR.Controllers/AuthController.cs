namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Security;
    using Core.Services;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IUserControlSession userControlSession;
        private readonly ISecurityService securityService;
        private readonly ICryptoService cryptoService;

        public AuthController(
            ISessionService sessionService,
            IAuthService authService,
            IUserService userService,
            IUserControlSession userControlSession,
            ISecurityService securityService,
            ICryptoService cryptoService)
        {
            this.sessionService = sessionService;
            this.authService = authService;
            this.userService = userService;
            this.userControlSession = userControlSession;
            this.securityService = securityService;
            this.cryptoService = cryptoService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var loginParam = new LoginParam
                {
                    Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
                    Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                    Token = token,
                    IP = Request.UserHostAddress,
                    Platform = Request.Headers["User-Agent"],
                    BrowserName = Request.Headers["User-Agent"]
                };
                var loginResult = securityService.Login(loginParam);
                if (loginResult.IsSuccessfull)
                {
                    userControlSession.UserId = loginResult.UserId;
                    userControlSession.JwtToken = loginResult.Token;
                    userControlSession.MenuItems = userService.GetMenuItem();
                    userControlSession.Functions = userService.GetFunctionPermission();
                    //HRM session
                    EmployeeSession employee = authService.GetEmployeeByCode(model.Username);
                    sessionService.IsShowLeftNavigation = true;
                    sessionService.Employee = employee;
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            sessionService.ClearAll();
            userControlSession.ClearAll();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}