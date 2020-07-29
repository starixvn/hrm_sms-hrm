namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Log;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;

    public class AccountController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IAuthService authService;
        private readonly IEmployeeLogService employeeLogService;

        public AccountController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IEmployeeLogService employeeLogService,
            IAuthService authService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.authService = authService;
            this.employeeLogService = employeeLogService;
        }

        public ActionResult Index()
        {
            Employee model = employeeService.GetEmployeeById(sessionService.Employee.Id);

            return View(model);
        }

        [HttpPost]
        public ActionResult HistoryDetail(int id)
        {
            IEnumerable<LogObjectDetail> logs = employeeLogService.GetLogDetailById(id);

            return PartialView("_HistoryDetail", logs);
        }

        [HttpPost]
        public ActionResult EditPasswordResetStatus(bool status)
        {
            CommandResult commandResult = employeeService.EditPasswordResetStatus(sessionService.Employee.Id, status);
            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ResetPassword(int id)
        {
            var model = new EditPassword
            {
                EmployeeId = id,
                Password = Encryptor.ComputeHash(authService.HashProvider, "123456"),
                IsReset = true
            };
            CommandResult commandResult = employeeService.EditPassword(sessionService.Employee.Id, model);
            return Json(commandResult);
        }

        [HttpGet]
        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                int retCode = authService.Validate(sessionService.Employee, model.Password);
                if (retCode == (short)ValidateUserStatus.Success)
                {
                    var param = new EditPassword
                    {
                        EmployeeId = sessionService.Employee.Id,
                        Password = Encryptor.ComputeHash(authService.HashProvider, model.NewPassword)
                    };
                    CommandResult commandResult = employeeService.EditPassword(sessionService.Employee.Id, param);
                    switch (commandResult.Code)
                    {
                        case (short)ValidateUserStatus.Success: // OK
                            ViewBag.ChangePwdOK = Messages.ChangePwdOK;
                            break;

                        case (short)ValidateUserStatus.Deleted:
                            ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                            break;

                        case (short)ValidateUserStatus.Disabled:
                            ModelState.AddModelError(string.Empty, Messages.DisabledUser);
                            break;

                        default: // wrong password
                            ModelState.AddModelError(string.Empty, Messages.WrongPassword);
                            break;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.WrongPassword);
                }
            }

            return View(model);
        }
    }
}