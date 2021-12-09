namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class AccountController : BaseController
    {
        private readonly IAuthService authService;
        private readonly IEmployeeService employeeService;
        private readonly IEmployeeLogService employeeLogService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public AccountController(
            IUserControlSession userControlSession,
            IAuthService authService,
            IEmployeeService employeeService,
            IEmployeeLogService employeeLogService,
            ICryptoService cryptoService,
            IUserService userService) : base(userControlSession)
        {
            this.authService = authService;
            this.employeeService = employeeService;
            this.employeeLogService = employeeLogService;
            this.cryptoService = cryptoService;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            var viewModel = new AccountViewModel
            {
                Employee = employeeService.GetEmployeeById(EmployeeSession.Id),
                Logs = employeeLogService.GetLogDetailById(EmployeeSession.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ResetPassword(EditPassword model)
        {
            model.Password = Encryptor.ComputeHash(authService.HashProvider, "123456");
            model.IsReset = true;
            var commandResult = employeeService.EditPassword(EmployeeSession.Id, model);
            if (commandResult.IsSuccessful)
            {
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var result = userService.ResetPassword(new UpdatePasswordParam
                {
                    UserId = model.UserId ?? 0,
                    Password = cryptoService.Encrypt("123456", rijndaelKey, rijndaelIV),
                    Token = token
                });
                return Json(result);
            }

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
                int retCode = authService.Validate(EmployeeSession, model.Password);
                if (retCode == (short)ValidateUserStatus.Success)
                {
                    var param = new EditPassword
                    {
                        EmployeeId = EmployeeSession.Id,
                        Password = Encryptor.ComputeHash(authService.HashProvider, model.NewPassword)
                    };
                    var commandResult = employeeService.EditPassword(EmployeeSession.Id, param);
                    switch (commandResult.Code)
                    {
                        case (short)ValidateUserStatus.Success: // OK
                            UpdatePassword(model);
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

        private void UpdatePassword(ChangePasswordViewModel model)
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var commandResult = userService.UpdatePassword(new UpdatePasswordParam
            {
                OldPassword = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(model.NewPassword, rijndaelKey, rijndaelIV),
                Token = token
            });
            if (commandResult.IsSuccessful)
            {
                ViewBag.ChangePwdOK = Messages.ChangePwdOK;
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }
    }
}