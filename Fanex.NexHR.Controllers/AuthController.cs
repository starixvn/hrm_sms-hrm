namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Employee;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AuthController()
        {
            _employeeService = new EmployeeService();
        }

        public AuthController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
                int retCode = _employeeService.ValidateUser(model.UserName, model.Password);
                switch (retCode)
                {
                    case 0: // OK
                        var criteria = new GetEmployeeByCodeCriteria() { Code = model.UserName };
                        UserLoginModel user = _employeeService.GetUserLoginByCode(criteria);
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

                    case -1:
                    case 3:
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
    }
}
