// --------------------------------------------------------------------------
// <copyright file="ChangeShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// ChangeShift Controller.
    /// </summary>
    [Authorize]
    public class ChangeShiftController : RequestController
    {
        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Input year.</param>
        /// <returns>View Employee according to year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.ChangeShift,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                0,
                true,
                this.CurrentUser.Id,
                null,
                null);

            return View(entityList);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var currentDate = DateTime.Now.Date;
            var listUnavailDates = GetUnavailableDateList(this.CurrentUser.Id);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() 
            { 
                Empid = this.CurrentUser.Id 
            };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var model = new ChangeShiftViewModel
            {
                Employee = this.CurrentUser.NickName,
                EmployeeId = employee.Id,
                FromDate = currentDate,
                ToDate = currentDate,
                By = this.CurrentUser.Id,
                ShiftDetails = new List<ShiftDetailsViewModel>(),
            };
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                RequestType = (int)RequestFilter.LateInEarlyOut,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = _RequestService.GetNumberRequestInMonth(requestInMonthCriteria);
            GetVerifierApproverList(this.CurrentUser.Id, model);
            ViewBag.TotalRequest = numberRequests;
            ViewBag.UnavailDates = listUnavailDates;
            
            return View(model);
        }

        /// <summary>
        /// Create a new change shift.
        /// </summary>
        /// <param name="shiftJson">The shift data.</param>
        /// <param name="shiftDetailJson">The shift detail data.</param>
        /// <returns>Result to check success.</returns>
        [HttpPost]
        public JsonResult Create(string shiftJson, string shiftDetailJson)
        {
            var shift = JsonConverter.JsonExtensionMethod.FromJson<ChangeShiftViewModel>(shiftJson);
            shift.ShiftDetails = JsonConverter.JsonExtensionMethod.FromJsonArray<ShiftDetailsViewModel>(shiftDetailJson);
            shift.By = this.CurrentUser.Id;
            var insert = _ChangeShiftRequestService.Insert(ModelState, shift);

            if (insert == false)
            {
                foreach (var item in ModelState.Values)
                {
                    foreach (ModelError error in item.Errors)
                    {
                        return Json(new { success = false, errors = error.ErrorMessage });
                    }
                }
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">Working time request's id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(int id)
        {
            var listUnavailDates = _EmployeeService.GetUnavailableDates(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var changeShift = _ChangeShiftRequestService.GetRequestById(id);
            if (changeShift == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, changeShift.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var shiftDetails = _ChangeShiftRequestService.GetChangeShiftDetailsById(id);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = changeShift.Requester.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var model = new ChangeShiftViewModel();
            if (changeShift != null && shiftDetails != null)
            {
                var lstShiftDeatails = new List<ShiftDetailsViewModel>();
                shiftDetails.ForEach(x => lstShiftDeatails.Add(new ShiftDetailsViewModel
                {
                    Date = x.OnDate.ToString("MM/dd/yyyy"),
                    FromShift = x.FromShiftID,
                    ToShift = x.ToShiftID,
                    ScheduleStatus = x.FromShiftID == -1 ? Labels.Off : x.FromShift,
                    ChangeToShiftCode = x.ToShift,
                    Remark = x.Remark,
                    ShiftId = x.ToShiftID,
                    HasData = true
                }));

                model = new ChangeShiftViewModel
                {
                    Employee = employee.NickName,
                    DisplayName = employee.DisplayName,
                    EmployeeId = employee.Id,
                    FromDate = changeShift.FromDate.Date,
                    ToDate = changeShift.ToDate.Date,
                    VerifierId = changeShift.VerifiedBy.Id,
                    ApproverId = changeShift.ApprovedBy.Id,
                    Remark = changeShift.Remark,
                    ShiftDetails = lstShiftDeatails
                };
            }

            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            GetVerifierApproverList(employee.Id, model);
            return View(model);
        }

        [HttpPost]
        public JsonResult Update(string shiftJson, string shiftDetailJson, int requestId)
        {
            var changeShift = JsonConverter.JsonExtensionMethod.FromJson<ChangeShiftViewModel>(shiftJson);
            changeShift.ShiftDetails = JsonConverter.JsonExtensionMethod.FromJsonArray<ShiftDetailsViewModel>(shiftDetailJson);
            changeShift.By = this.CurrentUser.Id;
            changeShift.Id = requestId;

            var result = _ChangeShiftRequestService.Update(ModelState, changeShift);
            return Json(new { success = result });
        }

        /// <summary>
        /// Changes the shift details.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Change Shift To date.</param>
        /// <returns>Action Result.</returns>
        public ActionResult ChangeShiftDetails(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var model = new List<ShiftDetailsViewModel>();
            var workingCalendars = _WorkingCalendarService.GetWorkingCalendarsByDateRange(fromDate, toDate, string.Empty, employeeId);
            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;

            foreach (WorkingCalendarModel item in workingCalendars)
            {
                var shiftDetail = new ShiftDetailsViewModel
                {
                    Date = item.WorkDate.ToString("ddd, MMM d, yyyy"),
                    Remark = string.Empty,
                    ScheduleStatus = GetScheduleStatus(item),
                    FromShift = item.IsOffDay ? -1 : (item.Shift != null ? item.Shift.Id : 0),
                    HasData = false
                };
                model.Add(shiftDetail);
            }

            return PartialView("ChangeShiftDetails", model);
        }

        /// <summary>
        /// Function to get schedule status.
        /// </summary>
        /// <param name="model">WorkingCalendar Model.</param>
        /// <returns>Schedule status.</returns>
        private string GetScheduleStatus(WorkingCalendarModel model)
        {
            var schedule = string.Empty;
            if (model.IsOffDay)
            {
                schedule = Labels.Off;
            }
            else if (!model.IsHoliday && !model.IsLeaveDay && !model.IsKeepInViewDay && !model.IsChangeShift && model.Shift != null)
            {
                schedule = model.Shift.Name;
            }
            else
            {
                schedule = Labels.NA;
            }

            return schedule;
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, ChangeShiftViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Verifier, (byte)RequestTypes.ChangeShift);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Approver, (byte)RequestTypes.ChangeShift);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        private IEnumerable<DateTime> GetUnavailableDateList(int id)
        {
            var previousMonth = DateTime.Now.AddMonths(-1).Date;
            var nextMonth = DateTime.Now.AddMonths(1).Date;
            var unvailableFromDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var unavailableToDate = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            var listUnavailDates = _EmployeeService.GetUnavailableDates(id, unvailableFromDate, unavailableToDate);
            return listUnavailDates;
        }
    }
}
