namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;

    public class AccountController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly IAuthService authService;

        public AccountController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IBaseService baseService,
            IAuthService authService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
            this.authService = authService;
        }

        public ActionResult Index()
        {
            Employee model = employeeService.GetEmployeeById(sessionService.Employee.Id);
            model.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            EmployeeShift employeeShift = employeeService.GetShiftByEmployeeId(sessionService.Employee.Id);

            ViewBag.IsUpdated = false;
            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            DisplayShiftForPersonalInformation(employeeShift);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Employee employeeModel)
        {
            employeeModel.Id = sessionService.Employee.Id;
            employeeModel.Modifier = new Employee { Id = sessionService.Employee.Id };
            ModelState.Clear();
            UpdateEmployeePersonalInformationCriteria criteria = SetUpdatePersonalInfoCriteria(employeeModel);
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                ViewBag.IsUpdated = true;
            }
            else
            {
                ModelState.AddModelError(result.Code.ToString(), result.Message);
                ViewBag.IsUpdated = false;
            }

            Employee displayModel = employeeService.GetEmployeeById(sessionService.Employee.Id);
            displayModel.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            var workingCalendar = employeeService.GetShiftByEmployeeId(sessionService.Employee.Id);

            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            DisplayShiftForPersonalInformation(workingCalendar);

            return View(displayModel);
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

        private void DisplayShiftForPersonalInformation(EmployeeShift workingCalendar)
        {
            ViewBag.Sun = workingCalendar.SunShiftName;
            ViewBag.Mon = workingCalendar.MonShiftName;
            ViewBag.Tue = workingCalendar.TueShiftName;
            ViewBag.Wed = workingCalendar.WedShiftName;
            ViewBag.Thu = workingCalendar.ThuShiftName;
            ViewBag.Fri = workingCalendar.FriShiftName;
            ViewBag.Sat = workingCalendar.SatShiftName;
        }

        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            IList<Country> countries = employeeService.GetAllCountry();
            ViewBag.Countries = countries.Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryCode }).ToList();

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = Labels.Female,
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = Labels.Male,
                    Value = "1"
                }
            };

            ViewBag.Marriages = Utilities.Marriages;
        }

        private UpdateEmployeePersonalInformationCriteria SetUpdatePersonalInfoCriteria(Employee employeeModel)
        {
            var criteria = new UpdateEmployeePersonalInformationCriteria
            {
                EmpId = employeeModel.Id,
                DateOfBirth = employeeModel.DateOfBirth,
                PlaceOfbirth = employeeModel.PlaceOfBirth,
                Ethnic = employeeModel.Ethnic,
                Native = employeeModel.Native,
                Religion = employeeModel.Religion,
                DateOfIdCard = employeeModel.DateOfIdCard,
                PlaceOfIdCard = employeeModel.PlaceOfIdCard,
                HomePhone = employeeModel.HomePhone,
                Qualification = employeeModel.Qualification,
                Major = employeeModel.Major,
                Phone = employeeModel.Phone,
                Gender = employeeModel.Gender,
                Skype = employeeModel.Skype,
                Yahoo = employeeModel.LetsTalk,
                ChineseName = employeeModel.ChineseName,
                Marriage = employeeModel.Marriage,
                PermanentAddress = employeeModel.Address,
                CurrentAddress = employeeModel.CurrentAddress,
                IdCardNumber = employeeModel.CardNo,
                PassportNo = employeeModel.PassportNo,
                PassportExp = employeeModel.PassportExp,
                ModifiedBy = employeeModel.Modifier.Id,
                CountryCode = employeeModel.CountryCode
            };

            return criteria;
        }
    }
}