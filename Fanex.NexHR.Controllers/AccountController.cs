namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Employee;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class AccountController : BaseController
    {
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public AccountController()
            : this(null, null, null)
        {
        }

        public AccountController(
            IEmployeeService employeeService,
            ILookupDataService lookupDataService,
            ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var id = CurrentUser.Id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var model = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            model.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = model.JobTitleId == null
            ? Resources.Labels.Unknown
            : lookupDataService.GetLookupByCodeAndId(Constants.JobTitle, model.JobTitleId.Value).Name;
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = employeeService.GetMySpace(getMySpaceCriteria);
            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            var workingCalendar = employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = model.CountryCode };
            var countryName = employeeService.GetCountryByCode(getCountryByCodeCriteria);
            ViewBag.CountryName = countryName.CountryName;
            ViewBag.IsUpdated = false;
            this.DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            this.DisplayShiftForPersonalInformation(workingCalendar);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Employee employeeModel)
        {
            var id = this.CurrentUser.Id;
            employeeModel.Id = id;
            employeeModel.Modifier = new Employee { Id = id };
            ModelState.Clear();

            UpdateEmployeePersonalInformationCriteria criteria = SetUpdatePersonalInfoCriteria(employeeModel);
            CommandResult result = commandService.Update(criteria);
            if (result.Code != 0)
            {
                ModelState.AddModelError(result.Code.ToString(), result.Message);
                ViewBag.IsUpdated = false;
            }
            else
            {
                ViewBag.IsUpdated = true;
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var displayModel = this.employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            displayModel.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = displayModel.JobTitleId == null
            ? Resources.Labels.Unknown
            : lookupDataService.GetLookupByCodeAndId(Constants.JobTitle, displayModel.JobTitleId.Value).Name;

            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = employeeService.GetMySpace(getMySpaceCriteria);
            ViewBag.DefaultCalendar = displayModel.IsDefaultSchedule;

            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            var workingCalendar = employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = displayModel.CountryCode };
            var countryName = employeeService.GetCountryByCode(getCountryByCodeCriteria);
            ViewBag.CountryName = countryName.CountryName;

            this.DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            this.DisplayShiftForPersonalInformation(workingCalendar);

            return View(displayModel);
        }

        public int UpdatePasswordResetStatus(bool status)
        {
            var criteria = new UpdatePasswordResetStatusCriteria()
            {
                EmpId = CurrentUser.Id,
                PasswordReset = status
            };
            CommandResult result = commandService.Update(criteria);

            return result.Code;
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public int ResetPassword(int id)
        {
            string hashPassword = Encryptor.ComputeHash(employeeService.HashProvider, "123456");
            var criteria = new ChangePasswordCriteria()
            {
                EmpId = id,
                Password = hashPassword,
                PasswordReset = true,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = commandService.Update(criteria);
            return result.Code;
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
                int retCode = employeeService.ValidateUser(CurrentUser.Code, model.Password);
                if (retCode == 0)
                {
                    var criteria = new ChangePasswordCriteria()
                    {
                        EmpId = CurrentUser.Id,
                        Password = Encryptor.ComputeHash(employeeService.HashProvider, model.NewPassword)
                    };

                    CommandResult result = commandService.Update(criteria);
                    switch (result.Code)
                    {
                        case 0: // OK
                            ViewBag.ChangePwdOK = Messages.ChangePwdOK;
                            break;

                        case 1: // deleted
                            ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                            break;

                        case 2: // disabled
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

        /// <summary>
        /// Displays the shift for personal information.
        /// </summary>
        /// <param name="workingCalendar">The working calendar.</param>
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

        /// <summary>
        /// Displays the default values of drop down lists for personal information.
        /// </summary>
        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            var getAllCountryCriteria = new GetAllCountryCriteria();
            var countries = employeeService.GetAllCountry(getAllCountryCriteria);
            ViewBag.Countries = countries.Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryCode }).ToList();

            ViewBag.Genders = new List<SelectListItem>()
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

        /// <summary>
        /// Set Update Personal Information Criteria.
        /// </summary>
        /// <param name="employeeModel">Employee Model.</param>
        /// <returns>UpdateEmployeePersonalInformation Criteria.</returns>
        private UpdateEmployeePersonalInformationCriteria SetUpdatePersonalInfoCriteria(Employee employeeModel)
        {
            var updatePersonalInformationCriteria = new UpdateEmployeePersonalInformationCriteria
            {
                EmpId = employeeModel.Id,
                DateOfBirth = employeeModel.DateOfBirth,
                PlaceOfbirth = employeeModel.PlaceOfBirth,
                Ethnic = employeeModel.Ethnic,
                Native = employeeModel.Native,
                Religion = employeeModel.Religion,
                DateOfIDCard = employeeModel.DateOfIdCard,
                PlaceOfIDCard = employeeModel.PlaceOfIdCard,
                HomePhone = employeeModel.HomePhone,
                Qualification = employeeModel.Qualification,
                Major = employeeModel.Major,
                Phone = employeeModel.Phone,
                Gender = employeeModel.Gender,
                Skype = employeeModel.Skype,
                Yahoo = employeeModel.Yahoo,
                ChineseName = employeeModel.ChineseName,
                Marriage = employeeModel.Marriage,
                PermanentAddress = employeeModel.Address,
                CurrentAddress = employeeModel.CurrentAddress,
                IdCardNumber = employeeModel.CardNo,
                PassportNo = employeeModel.PassportNo,
                PassportExp = employeeModel.PassportExp,
                ModifiedBy = employeeModel.Modifier.Id,
                ModifiedDate = DateTime.Now,
                CountryCode = employeeModel.CountryCode
            };

            return updatePersonalInformationCriteria;
        }
    }
}