namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Security;
    using Core.Services;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;

    public class AuthController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;

        public AuthController(
            IBaseService baseService,
            ISessionService sessionService,
            IAuthService authService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.authService = authService;
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
                EmployeeSession employee = authService.GetEmployeeByCode(model.UserName);
                int retCode = authService.Validate(employee, model.Password);
                switch (retCode)
                {
                    case (short)ValidateUserStatus.Success:
                        LogEmployeeLogin(employee.Id);
                        sessionService.IsShowLeftNavigation = true;
                        sessionService.Employee = employee;
                        sessionService.MenuItems = authService.GetMenuByEmployeeId(employee.Id);
                        sessionService.Functions = authService.GetFunctionByEmployeeId(employee.Id);
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("Index", "Home");

                    case (short)ValidateUserStatus.Deleted:
                        ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                        break;

                    case (short)ValidateUserStatus.Disabled:
                        ModelState.AddModelError(string.Empty, Messages.DisabledUser);
                        break;

                    default: // wrong username/password
                        ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
                        break;
                }
            }

            return View(model);
        }

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