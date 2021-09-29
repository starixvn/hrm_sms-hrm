namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Security;
    using Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.ViewModels.Employee;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services.Interfaces;
    using HRM.UserControl.Client.Core.Session;

    public class AuthController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;
        private readonly IUCSession ucSession;
        private readonly ISecurityService securityService;

        public AuthController(
            IBaseService baseService,
            ISessionService sessionService,
            IUserService userService,
            IUCSession ucSession,
            ISecurityService securityService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.userService = userService;
            this.ucSession = ucSession;
            this.securityService = securityService;
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
                var user = userService.GetByEmail(model.UserName);
                if (user != null)
                {
                    var loginResult = LoginUserControl(user);
                    if (loginResult.IsLoginSuccess)
                    {
                        ucSession.UserId = loginResult.UserId;
                        ucSession.OnCompanyId = loginResult.CompanyId;
                        ucSession.JwtToken = loginResult.Token;
                    }
                }
            }
            return View(model);
        }

        private async Task<IList<CompanyRoleFunction>> GetUserRolesOnCompany()
        {
            var roles = await userService.GetPermissionAsync();

            return roles;
        }

        private LoginResult LoginUserControl(User user)
        {
            var loginParam = new LoginParam
            {
                UserId = user.UserId,
                CompanyId = 2,
                Ip = Request.UserHostAddress,
                Platform = Request.Headers["User-Agent"],
                BrowserName = Request.Headers["User-Agent"]
            };
            var loginResult = securityService.Login(loginParam);

            return loginResult;
        }

        private static bool IsValidPassword(string plainPwd, string encryptedPwd)
        {
            return true;
        }

        //[HttpPost]
        //public ActionResult Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
        //        EmployeeSession employee = authService.GetEmployeeByCode(model.UserName);
        //        int retCode = authService.Validate(employee, model.Password);
        //        switch (retCode)
        //        {
        //            case (short)ValidateUserStatus.Success:
        //                LogEmployeeLogin(employee.Id);
        //                sessionService.IsShowLeftNavigation = true;
        //                sessionService.Employee = employee;
        //                sessionService.MenuItems = authService.GetMenuByEmployeeId(employee.Id);
        //                sessionService.Functions = authService.GetFunctionByEmployeeId(employee.Id);
        //                FormsAuthentication.SetAuthCookie(model.UserName, false);
        //                return RedirectToAction("Index", "Home");

        //            case (short)ValidateUserStatus.Deleted:
        //                ModelState.AddModelError(string.Empty, Messages.DeletedUser);
        //                break;

        //            case (short)ValidateUserStatus.Disabled:
        //                ModelState.AddModelError(string.Empty, Messages.DisabledUser);
        //                break;

        //            default: // wrong username/password
        //                ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
        //                break;
        //        }
        //    }

        //    return View(model);
        //}

        [HttpGet]
        public ActionResult Logout()
        {
            sessionService.ClearAll();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        private void LogEmployeeLogin(int employeeId)
        {
            var criteria = new LogEmployeeLoginCriteria
            {
                EmployeeId = employeeId,
                IP = Request.UserHostAddress,
                Platform = Request.Headers["User-Agent"],
                LoginTime = DateTime.Now
            };

            baseService.ExecuteNonQuery(criteria);
        }
    }
}