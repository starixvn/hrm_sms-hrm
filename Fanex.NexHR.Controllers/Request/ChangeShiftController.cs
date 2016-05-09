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
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.Request;

    [Authorize]
    public class ChangeShiftController : RequestController
    {
        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            var criteria = new GetRequestCriteria()
            {
                EmpId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.ChangeShift,
                IsQueryByCreatedDate = false,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                SupervisorId = CurrentUser.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = RequestService.GetRequests(criteria);

            return View(entityList);
        }

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
                RequestType = (int)RequestType.ChangeShift,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = RequestService.GetNumberRequestInMonth(requestInMonthCriteria);
            GetVerifierApproverList(this.CurrentUser.Id, model);
            var shifts = ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            ViewBag.TotalRequest = numberRequests;
            ViewBag.UnavailDates = listUnavailDates;

            return View(model);
        }

        [HttpPost]
        public JsonResult Create(string shiftJson, string shiftDetailJson)
        {
            var criteria = JsonConverter.FromJson<InsertChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.CreatedBy = this.CurrentUser.Id;
            criteria.CreatedDate = DateTime.Now;

            CommandResult result = ChangeShiftRequestService.Insert(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int id)
        {
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            ViewBag.UnavailDates = listUnavailDates;
            var getRequestByIdcriteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            var request = RequestService.GetRequestById(getRequestByIdcriteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria()
            {
                RequestId = id
            };
            var changeShiftDetail = ChangeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);
            var model = new ChangeShiftViewModel();
            if (request != null && changeShiftDetail != null)
            {
                model = new ChangeShiftViewModel
                {
                    RequestId = request.Id,
                    Employee = this.CurrentUser.NickName,
                    EmployeeId = this.CurrentUser.Id,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    Remark = request.Remark,
                    ShiftDetails = ConvertChangeShiftDetailToViewModel(changeShiftDetail)
                };
            }

            GetVerifierApproverList(this.CurrentUser.Id, model);
            var shifts = ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            ViewBag.IsView = false;
            return View(model);
        }

        [HttpPost]
        public JsonResult Update(string shiftJson, string shiftDetailJson)
        {
            var criteria = JsonConverter.FromJson<UpdateChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.ModifiedBy = this.CurrentUser.Id;
            criteria.ModifiedDate = DateTime.Now;

            var result = ChangeShiftRequestService.Update(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var getRequestByIdcriteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            var request = RequestService.GetRequestById(getRequestByIdcriteria);
            var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria()
            {
                RequestId = id
            };
            IList<ChangeShiftDetailModel> changeShiftDetail = ChangeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.Requester.Id };
            var employee = EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
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
            var shifts = ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            ViewBag.IsView = true;
            return View(model);
        }

        public ActionResult ChangeShiftDetail(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var model = GetChangeShiftDetail(employeeId, fromDate, toDate);
            var shifts = ShiftService.GetAll(isShowOff: true);
            ViewBag.Shift = shifts;
            return PartialView("ChangeShiftDetail", model);
        }

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
            IList<WorkingCalendarModel> calendars = WorkingCalendarService.GetWorkingCalendarByDateRange(criteria);
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

        private void GetVerifierApproverList(int id, ChangeShiftViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Verifier, (byte)RequestType.ChangeShift);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Approver, (byte)RequestType.ChangeShift);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

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
            var listUnavailDates = EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            return listUnavailDates;
        }

        private IList<ChangeShiftDetailViewModel> ConvertChangeShiftDetailToViewModel(IList<ChangeShiftDetailModel> model)
        {
            return model.Select(item => new ChangeShiftDetailViewModel()
            {
                OnDate = item.OnDate,
                ScheduleStatus = item.FromShiftId == -1 ? Labels.Off : item.FromShift,
                FromShiftId = item.FromShiftId,
                ToShiftId = item.ToShiftId, Remark = item.Remark
            }).ToList();
        }
    }
}