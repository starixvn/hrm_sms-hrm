namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;

    public class AccountController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IEmployeeLogService employeeLogService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public AccountController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IEmployeeLogService employeeLogService,
            ICryptoService cryptoService,
            IUserService userService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.employeeLogService = employeeLogService;
            this.cryptoService = cryptoService;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            var viewModel = new AccountViewModel
            {
                Employee = employeeService.GetEmployeeById(sessionService.Employee.Id),
                Logs = employeeLogService.GetLogDetailById(sessionService.Employee.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ResetPassword(int userId)
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            CommandResult commandResult = userService.ResetPassword(new UpdatePasswordParam
            {
                UserId = userId,
                Password = cryptoService.Encrypt("123456", rijndaelKey, rijndaelIV),
                Token = token
            });
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
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                CommandResult commandResult = userService.UpdatePassword(new UpdatePasswordParam
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

            return View(model);
        }
    }
}