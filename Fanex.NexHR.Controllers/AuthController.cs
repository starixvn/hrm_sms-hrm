namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Employee;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public AuthController()
            : this(null, null)
        {
        }

        public AuthController(IEmployeeService employeeService, ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.commandService = commandService ?? new CommandService();
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
                int retCode = employeeService.ValidateUser(model.UserName, model.Password);
                switch (retCode)
                {
                    case 0: // OK
                        var criteria = new GetEmployeeByCodeCriteria { Code = model.UserName };
                        UserLoginModel user = employeeService.GetUserLoginByCode(criteria);
                        LogEmployeeLogin(user.Id);
                        user.IsShowLeftNavigation = true;
                        HttpContext.Session["CurrentUser"] = user;
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("Index", "Home");

                    case 1: // deleted
                        ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                        break;

                    case 2: // disalbed
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
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Abandon();
                HttpContext.Session.RemoveAll();
            }

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

            commandService.Insert(criteria);
        }
    }
}