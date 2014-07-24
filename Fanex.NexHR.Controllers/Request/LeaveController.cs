// --------------------------------------------------------------------------
// <copyright file="LeaveController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Leave Controller.
    /// </summary>
    public class LeaveController : RequestController
    {
        /// <summary>
        /// ActionResult Create leave request.
        /// </summary>
        /// <returns>View Result.</returns>
        public ActionResult Create()
        {
            Employee employee = _EmployeeService.GetEmployeeById(this.CurrentUser.Id);
            EmployeeAnnualBalance employeeBalance = _LeaveRequestService.GetAnnualLeaveAndKeepInViewBalance(this.CurrentUser.Id, null, DateTime.Now.Date);
            IList<LeaveType> leaveTypes = _LeaveTypeService.GetAll();
            IList<SupervisorNameModel> verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.Leave);
            IList<SupervisorNameModel> approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.Leave);
            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            decimal numberDays = _RequestService.GetNumberOfRequest((int)RequestFilter.Leave, this.CurrentUser.Id, fromDate, toDate);
            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = Constants.GeneralLeave,
                Employee = employee.DisplayName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                By = this.CurrentUser.Id,
                LeaveTypeList = leaveTypes,
                TotalRequest = numberDays,
                VerifierList = GetListItem(verifiers),
                ApproverList = GetListItem(approvers),
                EmployeeBalance = employeeBalance
            };

            return View(model);
        }

        /// <summary>
        /// Function create new request.
        /// </summary>
        /// <param name="request">Format request string.</param>
        /// <param name="requestDetails">Format request detail string.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult Create(string request, string requestDetails)
        {
            var errorCode = 0;
            var message = string.Empty;
            LeaveRequestModel leaveRequest = JsonConverter.JsonExtensionMethod.FromJson<LeaveRequestModel>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.JsonExtensionMethod.FromJsonArray<LeaveDetailModel>(requestDetails);
            leaveRequest.RequestDetail = leaveRequestDetail;
            leaveRequest.CreatedBy = this.CurrentUser.Id;

            _LeaveRequestService.Insert(leaveRequest, out errorCode, out message);

            return Json(new { errorCode = errorCode, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="requestId">WorkingTimeRequest id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(long requestId)
        {
            LeaveRequestViewModel leaveRequest = _LeaveRequestService.GetRequestById(requestId);
            if (leaveRequest == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, leaveRequest.EmployeeId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            EmployeeAnnualBalance employeeBalance = _LeaveRequestService.GetAnnualLeaveAndKeepInViewBalance(this.CurrentUser.Id, requestId, DateTime.Now.Date);
            IList<LeaveType> leaveTypes = _LeaveTypeService.GetAll();
            IList<SupervisorNameModel> verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.Leave);
            IList<SupervisorNameModel> approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.Leave);
            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
            minDate = minDate > leaveRequest.FromDate ? leaveRequest.FromDate : minDate;
            if (leaveRequest.LeaveTypeId == Constants.MaternityLeave)
            {
                //// rule: user can make leave request (maternity leave) for 6 months.
                maxDate = maxDate.AddMonths(2);
            }

            IEnumerable<DateTime> listUnavailDates = leaveRequest.LeaveTypeId == Constants.GeneralLeave
                ? null
                : _EmployeeService.GetUnavailableDates(this.CurrentUser.Id, minDate, maxDate);

            leaveRequest.UnavailDates = listUnavailDates;
            leaveRequest.LeaveTypeList = leaveTypes;
            leaveRequest.VerifierList = GetListItem(verifiers);
            leaveRequest.ApproverList = GetListItem(approvers);
            leaveRequest.EmployeeBalance = employeeBalance;

            ViewBag.IsView = false;
            ViewBag.MinDate = minDate;
            ViewBag.MaxDate = maxDate;
            return View(leaveRequest);
        }

        /// <summary>
        /// Function update leave request.
        /// </summary>
        /// <param name="request">Format request string.</param>
        /// <param name="requestDetails">Format request detail string.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string request, string requestDetails)
        {
            var errorCode = 0;
            var message = string.Empty;
            LeaveRequestModel leaveRequest = JsonConverter.JsonExtensionMethod.FromJson<LeaveRequestModel>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.JsonExtensionMethod.FromJsonArray<LeaveDetailModel>(requestDetails);
            leaveRequest.RequestDetail = leaveRequestDetail;
            leaveRequest.ModifiedBy = this.CurrentUser.Id;

            _LeaveRequestService.Update(leaveRequest, out errorCode, out message);

            return Json(new { errorCode = errorCode, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Detail.
        /// </summary>
        /// <param name="requestId">WorkingTimeRequest id.</param>
        /// <returns>View Detail.</returns>
        public ActionResult Detail(int requestId)
        {
            IList<LeaveType> leaveTypes = _LeaveTypeService.GetAll();
            IList<SupervisorNameModel> verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.Leave);
            IList<SupervisorNameModel> approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.Leave);
            LeaveRequestViewModel leaveRequest = _LeaveRequestService.GetRequestById(requestId);

            leaveRequest.LeaveTypeList = leaveTypes;
            leaveRequest.VerifierList = GetListItem(verifiers);
            leaveRequest.ApproverList = GetListItem(approvers);

            ViewBag.IsView = true;
            return View(leaveRequest);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Inputted year.</param>
        /// <returns>View Employee's Leave Request.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            EmployeeAnnualBalance employeeBalance = _LeaveRequestService.GetAnnualLeaveAndKeepInViewBalance(this.CurrentUser.Id, null, new DateTime(year, 1, 1));

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.Leave,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                0,
                true,
                this.CurrentUser.Id,
                null,
                null);

            ViewBag.Year = year;
            ViewBag.EmployeeBalance = employeeBalance;
            return View(entityList);
        }

        public ActionResult GetVerifiers(int employeeId, int requestType)
        {
            IList<SupervisorNameModel> verifierList = _EmployeeService.GetListSupervisorName(employeeId, (int)RolesEnum.Verifier, requestType);

            return Json(verifierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovers(int employeeId, int requestType)
        {
            IList<SupervisorNameModel> approverList = _EmployeeService.GetListSupervisorName(employeeId, (int)RolesEnum.Approver, requestType);

            return Json(approverList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime? toDate)
        {
            var effectDate = toDate.HasValue ? toDate.Value : DateTime.Now.Date;
            EmployeeAnnualBalance employeeBalance = _LeaveRequestService.GetAnnualLeaveAndKeepInViewBalance(employeeId, requestId, effectDate);

            return Json(employeeBalance, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action to get leave request detail base employeeId, FromDate, ToDate.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="requestId">Request Id.</param>
        /// <returns>Action Result.</returns>
        public ActionResult LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
        {
            IList<LeaveType> leaveTypes = _LeaveTypeService.GetAll();
            IList<WorkingCalendarModel> workingCalendars = _WorkingCalendarService.GetWorkingCalendarsByDateRange(fromDate, toDate, string.Empty, employeeId);
            IList<LeaveDetailModel> leaveRequestDetailList = new List<LeaveDetailModel>();
            if (requestId != -1)
            {
                leaveRequestDetailList = _LeaveRequestService.GetLeaveRequestDetailsById(requestId);
            }

            leaveRequestDetailList = ConvertToLeaveRequestDetail(workingCalendars, leaveRequestDetailList, fromDate, toDate);

            var model = new LeaveDetailViewModel
            {
                LeaveRequestDetail = leaveRequestDetailList,
                LeaveTypeList = leaveTypes
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SendReasonReeditToSupervisor(long requestId, string reason)
        {
            var errorMessage = string.Empty;
            var errorCode = 0;

            _LeaveRequestService.SendReasonReeditToSupervisor(requestId, this.CurrentUser.Id, reason, out errorCode, out errorMessage);

            return Json(new { errorCode = errorCode, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to convert from LeaveDetailModel get DB with WorkingCalendarModel.
        /// </summary>
        /// <param name="workingCalendars">List WorkingCalendarModel.</param>
        /// <param name="leaveRequestDetailInput">List LeaveDetailModel.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <returns>List LeaveDetail Model.</returns>
        [NonAction]
        private List<LeaveDetailModel> ConvertToLeaveRequestDetail(
            IList<WorkingCalendarModel> workingCalendars,
            IList<LeaveDetailModel> leaveRequestDetailInput,
            DateTime fromDate,
            DateTime toDate)
        {
            var leaveRequestDetailOutput = new List<LeaveDetailModel>();
            if (fromDate <= toDate)
            {
                var numberDays = (toDate - fromDate).Days + 1;
                var tempDate = DateTime.Now.Date;
                IEnumerable<WorkingCalendarModel> tempWokingCalendars = null;
                IEnumerable<LeaveDetailModel> tempLeaveDetailModels = null;
                WorkingCalendarModel tempWokingCalendar = null;

                for (int i = 0; i < numberDays; i++)
                {
                    tempLeaveDetailModels = null;
                    tempDate = fromDate.AddDays(i);
                    tempLeaveDetailModels = leaveRequestDetailInput.Where(t => (t.LeaveDate == tempDate));
                    tempWokingCalendars = workingCalendars.Where(t => t.WorkDate.Date == tempDate);

                    if (tempWokingCalendars != null && tempWokingCalendars.Any())
                    {
                        tempWokingCalendar = tempWokingCalendars.FirstOrDefault();
                        AddLeaveRequestExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempWokingCalendar);
                    }
                    else
                    {
                        AddLeaveRequestNotExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempDate);
                    }
                }
            }

            return leaveRequestDetailOutput.OrderBy(t => t.LeaveDate).ThenBy(t => t.FromTime).ToList();
        }

        /// <summary>
        /// Function to add a leave request detail with exist working calendar. 
        /// </summary>
        /// <param name="leaveRequestDetailOutput">Output List LeaveDetailModel.</param>
        /// <param name="tempLeaveDetailModels">Input List LeaveDetailModel.</param>
        /// <param name="tempWokingCalendar">WorkingCalendar Model.</param>
        [NonAction]
        private void AddLeaveRequestExistCalendar(
           List<LeaveDetailModel> leaveRequestDetailOutput,
           IEnumerable<LeaveDetailModel> tempLeaveDetailModels,
           WorkingCalendarModel tempWokingCalendar)
        {
            string tempScheduleStatus = string.Empty;
            LeaveDetailModel tempLeaveDetailModel = null;
            var isOffOrHoliday = tempWokingCalendar.IsOffDay || tempWokingCalendar.IsHoliday;

            if (tempWokingCalendar.IsOffDay)
            {
                tempScheduleStatus = Labels.OffDay;
            }
            else if (tempWokingCalendar.IsHoliday)
            {
                tempScheduleStatus = Labels.Holiday;
            }
            else if (tempWokingCalendar.Shift != null)
            {
                tempScheduleStatus = tempWokingCalendar.Shift.Name;
            }
            else if (!tempWokingCalendar.IsLeaveDay)
            {
                tempScheduleStatus = Labels.NA;
            }

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.IsOffOrHoliday = isOffOrHoliday;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = tempWokingCalendar.WorkDate,
                    ScheduleStatus = tempScheduleStatus,
                    IsOffOrHoliday = isOffOrHoliday,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        /// <summary>
        /// Function to add a leave request detail with not exist working calendar.
        /// </summary>
        /// <param name="leaveRequestDetailOutput">Output List LeaveDetailModel.</param>
        /// <param name="tempLeaveDetailModels">Input List LeaveDetailModel.</param>
        /// <param name="workDate">Input the work date.</param>
        [NonAction]
        private void AddLeaveRequestNotExistCalendar(
            List<LeaveDetailModel> leaveRequestDetailOutput, 
            IEnumerable<LeaveDetailModel> tempLeaveDetailModels, 
            DateTime workDate)
        {
            string tempScheduleStatus = Labels.NA;
            LeaveDetailModel tempLeaveDetailModel = null;

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = workDate,
                    ScheduleStatus = Labels.NA,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        [NonAction]
        private IList<SelectListItem> GetListItem(IList<SupervisorNameModel> supervisors)
        {
            IList<SelectListItem> selectItems = supervisors.Select(x => new SelectListItem 
            { 
                Text = x.NickName + " (" + x.FullName + ")", 
                Value = x.OwnerId.ToString() 
            }).ToList();

            return selectItems;
        }
    }
}
