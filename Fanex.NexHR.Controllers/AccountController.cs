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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
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
            var model = this._employeeService.GetEmployeeById(id);
            model.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = model.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;
            ViewBag.Working = _employeeService.GetMySpace(id);
            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            var workingCalendar = _employeeService.GetEmployeeShift(id);
            var countryName = _employeeService.GetCountryByCode(model.CountryCode);
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
            int errorCode = 0;
            string message = string.Empty;
            employeeModel.Id = this.CurrentUser.Id;
            employeeModel.Modifier = new Employee { Id = this.CurrentUser.Id };
            ModelState.Clear();
            ViewBag.IsUpdated = _employeeService.UpdatePersonalInformation(ModelState, employeeModel, out errorCode, out message);

            var id = this.CurrentUser.Id;
            var displayModel = this._employeeService.GetEmployeeById(id);
            displayModel.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;
            ViewBag.JobTitle = displayModel.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.Get(LookupCodeName.JobTitle, displayModel.JobTitleId.Value).Name;
            ViewBag.Working = _employeeService.GetMySpace(id);
            ViewBag.DefaultCalendar = displayModel.IsDefaultSchedule;
            var workingCalendar = _employeeService.GetEmployeeShift(id);
            var countryName = _employeeService.GetCountryByCode(displayModel.CountryCode);
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
            var countries = _employeeService.GetAllCountry();
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
    }
}