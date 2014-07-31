// --------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.MySpace;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Account Controller.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IShiftService _shiftService;
        private readonly ILookupDataService _lookupDataService;

        private readonly IEmployeeService _employeeService;

        public AccountController()
            : this(null, null, null)
        {
        }

        public AccountController(
            IEmployeeService employeeService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
        }

        /// <summary>
        /// Index Action.
        /// </summary>
        /// <returns>Action Result Index.</returns>
        public ActionResult Index()
        {
            var id = this.CurrentUser.Id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var model = this._employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            model.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = model.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.GetLookupByCodeAndId(LookupCodename.JobTitle, model.JobTitleId.Value).Name;
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = model.CountryCode };
            var countryName = _employeeService.GetCountryByCode(getCountryByCodeCriteria);
            ViewBag.CountryName = countryName.CountryName;

            this.DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            this.DisplayShiftForPersonalInformation(workingCalendar);

            return View(model);
        }

        /// <summary>
        /// Index Action.
        /// </summary>
        /// <param name="employeeModel">The employee model.</param>
        /// <returns>
        /// Action Result Index.
        /// </returns>
        [HttpPost]
        public ActionResult Index(Employee employeeModel)
        {
            string message = string.Empty;
            employeeModel.Id = this.CurrentUser.Id;
            employeeModel.Modifier = new Employee { Id = this.CurrentUser.Id };
            ModelState.Clear();
            UpdateEmployeePersonalInformationCriteria criteria = SetUpdatePersonalInfoCriteria(employeeModel);
            CommandResult result = _employeeService.UpdatePersonalInformation(criteria);
            if (result.Code != 0)
            {
                ModelState.AddModelError(result.Code.ToString(), result.Message);
                ViewBag.IsUpdated = false;
            }
            else
            {
                ViewBag.IsUpdated = true;
            }

            var id = this.CurrentUser.Id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var displayModel = this._employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            displayModel.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = displayModel.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.GetLookupByCodeAndId(LookupCodename.JobTitle, displayModel.JobTitleId.Value).Name;
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
            ViewBag.DefaultCalendar = displayModel.IsDefaultSchedule;
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = displayModel.CountryCode };
            var countryName = _employeeService.GetCountryByCode(getCountryByCodeCriteria);
            ViewBag.CountryName = countryName.CountryName;

            this.DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            this.DisplayShiftForPersonalInformation(workingCalendar);

            return View(displayModel);
        }

        /// <summary>
        /// Displays the shift for personal information.
        /// </summary>
        /// <param name="workingCalendar">The working calendar.</param>
        private void DisplayShiftForPersonalInformation(EmployeeShift workingCalendar)
        {
            var shifts = _shiftService.GetAll();

            ViewBag.Sun = workingCalendar.SunShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SunShift).Name;

            ViewBag.Mon = workingCalendar.MonShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.MonShift).Name;

            ViewBag.Tue = workingCalendar.TueShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.TueShift).Name;

            ViewBag.Wed = workingCalendar.WedShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.WedShift).Name;

            ViewBag.Thu = workingCalendar.ThuShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.ThuShift).Name;

            ViewBag.Fri = workingCalendar.FriShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.FriShift).Name;

            ViewBag.Sat = workingCalendar.SatShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SatShift).Name;
        }

        /// <summary>
        /// Displays the default values of drop down lists for personal information.
        /// </summary>
        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            var getAllCountryCriteria = new GetAllCountryCriteria();
            var countries = _employeeService.GetAllCountry(getAllCountryCriteria);
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