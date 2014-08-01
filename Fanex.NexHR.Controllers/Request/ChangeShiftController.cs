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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.Request;

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
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.ChangeShift,
                IsQueryByCreatedDate = false,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                SupervisorId = null,
                IsLocal = null
            };

            IList<RequestEntity> entityList = _RequestService.GetRequests(criteria);

            return View(entityList);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var currentDate = DateTime.Now.Date;
            IEnumerable<DateTime> listUnavailDates = GetUnavailableDate(this.CurrentUser.Id);
            List<ChangeShiftDetailViewModel> shiftDetails = GetChangeShiftDetail(this.CurrentUser.Id, currentDate, currentDate);
            var model = new ChangeShiftViewModel
            {
                Employee = this.CurrentUser.NickName,
                EmployeeId = this.CurrentUser.Id,
                FromDate = currentDate,
                ToDate = currentDate,
                ShiftDetails = shiftDetails
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
            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
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
            var criteria = JsonConverter.FromJson<InsertChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.CreatedBy = this.CurrentUser.Id;
            criteria.CreatedDate = DateTime.Now;

            CommandResult result = _ChangeShiftRequestService.Insert(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">Working time request's id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(int id)
        {
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            ViewBag.UnavailDates = listUnavailDates;
            var getRequestByIdcriteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            var request = _RequestService.GetRequestById(getRequestByIdcriteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria()
            {
                RequestId = id
            };
            var changeShiftDetail = _ChangeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.Requester.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var model = new ChangeShiftViewModel();
            if (request != null && changeShiftDetail != null)
            {
                model = new ChangeShiftViewModel
                {
                    Employee = employee.NickName,
                    EmployeeId = employee.Id,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    Remark = request.Remark,
                    ShiftDetails = ConvertChangeShiftDetailToViewModel(changeShiftDetail)
                };
            }

            GetVerifierApproverList(employee.Id, model);
            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            ViewBag.IsView = false;
            return View(model);
        }

        [HttpPost]
        public JsonResult Update(string shiftJson, string shiftDetailJson, int requestId)
        {
            var criteria = JsonConverter.FromJson<UpdateChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.EmpId = this.CurrentUser.Id;
            criteria.ModifiedBy = this.CurrentUser.Id;
            criteria.ModifiedDate = DateTime.Now;

            var result = _ChangeShiftRequestService.Update(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int Id)
        {
            var getRequestByIdcriteria = new GetRequestByIdCriteria()
            {
                RequestId = Id
            };
            var request = _RequestService.GetRequestById(getRequestByIdcriteria);
            var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria()
            {
                RequestId = Id
            };
            var changeShiftDetail = _ChangeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.Requester.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var model = new ChangeShiftViewModel();
            if (request != null && changeShiftDetail != null)
            {
                model = new ChangeShiftViewModel
                {
                    Employee = employee.NickName,
                    EmployeeId = employee.Id,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    Remark = request.Remark,
                    ShiftDetails = ConvertChangeShiftDetailToViewModel(changeShiftDetail),
                    Status = request.Status
                };
            }

            GetVerifierApproverList(employee.Id, model);
            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            ViewBag.IsView = true;
            return View(model);
        }

        /// <summary>
        /// Changes the shift details.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Change Shift To date.</param>
        /// <returns>Action Result.</returns>
        public ActionResult ChangeShiftDetail(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var model = GetChangeShiftDetail(employeeId, fromDate, toDate);
            var shifts = _ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            return PartialView("ChangeShiftDetail", model);
        }

        /// <summary>
        /// Function to get change shift detail.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <returns>List ChangeShiftDetail.</returns>
        private List<ChangeShiftDetailViewModel> GetChangeShiftDetail(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var changeShiftDetail = new List<ChangeShiftDetailViewModel>();
            var criteria = new GetWorkingCalendarByDateRangeCriteria()
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupIds = string.Empty,
                EmpId = employeeId
            };
            IList<WorkingCalendarModel> calendars = _WorkingCalendarService.GetWorkingCalendarByDateRange(criteria);
            foreach (WorkingCalendarModel item in calendars)
            {
                var shiftDetail = new ChangeShiftDetailViewModel
                {
                    OnDate = item.WorkDate,
                    Remark = string.Empty,
                    ScheduleStatus = GetScheduleStatus(item),
                    FromShiftId = item.IsOffDay ? -1 : (item.Shift != null ? item.Shift.Id : 0)
                };
                changeShiftDetail.Add(shiftDetail);
            }

            return changeShiftDetail;
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
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Verifier, (byte)RequestType.ChangeShift);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Approver, (byte)RequestType.ChangeShift);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        /// <summary>
        /// Get Unavailable dates.
        /// </summary>
        /// <param name="id">Employee Id.</param>
        /// <returns>List of unavailable dates.</returns>
        private IEnumerable<DateTime> GetUnavailableDate(int id)
        {
            var previousMonth = DateTime.Now.AddMonths(-1).Date;
            var nextMonth = DateTime.Now.AddMonths(1).Date;
            var unvailableFromDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var unavailableToDate = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = id,
                FromDate = unvailableFromDate,
                ToDate = unavailableToDate
            };
            var listUnavailDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            return listUnavailDates;
        }

        /// <summary>
        /// Function to convert model to view model.
        /// </summary>
        /// <param name="model">ChangeShiftDetail Model.</param>
        /// <returns>ChangeShiftDetail ViewModel.</returns>
        private IList<ChangeShiftDetailViewModel> ConvertChangeShiftDetailToViewModel(IList<ChangeShiftDetailModel> model)
        {
            var results = new List<ChangeShiftDetailViewModel>();
            foreach (ChangeShiftDetailModel item in model)
            {
                var changeShiftDetailViewModel = new ChangeShiftDetailViewModel()
                {
                    OnDate = item.OnDate,
                    ScheduleStatus = item.FromShiftId == -1 ? Labels.Off : item.FromShift,
                    FromShiftId = item.FromShiftId,
                    ToShiftId = item.ToShiftId,
                    Remark = item.Remark
                };

                results.Add(changeShiftDetailViewModel);
            }

            return results;
        }
    }
}
