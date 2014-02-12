// --------------------------------------------------------------------------
// <copyright file="RequestController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
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
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Request Controller.
    /// </summary>
    [Authorize]
    public class RequestController : BaseController
    {
        #region private services
        protected readonly IEmployeeService _EmployeeService;
        protected readonly IShiftService _ShiftService;
        protected readonly ILeaveTypeService _LeaveTypeService;
        protected readonly IRequestService _RequestService;
        protected readonly ITeamService _TeamService;
        protected readonly IKeepInViewRequestService _KeepInViewRequestService;
        protected readonly IChangeShiftRequestService _ChangeShiftRequestService;
        protected readonly ICheckInOutRequestService _CheckInOutRequestService;
        protected readonly ILateInEarlyOutRequestService _LateInEarlyOutRequestService;
        protected readonly ILeaveRequestService _LeaveRequestService;
        protected readonly IOvertimeRequestService _OvertimeService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public RequestController(
            IEmployeeService employeeService = null,
            IShiftService shiftService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null,
            ITeamService teamService = null,
            IKeepInViewRequestService keepInViewRequestService = null,
            IChangeShiftRequestService changeShiftRequestService = null,
            ICheckInOutRequestService checkInOutRequestService = null,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService = null,
            ILeaveRequestService leaveRequestService = null,
            IOvertimeRequestService overtimeService = null)
        {
            _EmployeeService = employeeService ?? new EmployeeService();
            _ShiftService = shiftService ?? new ShiftService();
            _LeaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _RequestService = requestService ?? new RequestService();
            _TeamService = teamService ?? new TeamService();
            _KeepInViewRequestService = keepInViewRequestService ?? new KeepInViewRequestService();
            _ChangeShiftRequestService = changeShiftRequestService ?? new ChangeShiftRequestService();
            _CheckInOutRequestService = checkInOutRequestService ?? new CheckInOutRequestService();
            _LateInEarlyOutRequestService = lateInEarlyOutRequestService ?? new LateInEarlyOutRequestService();
            _LeaveRequestService = leaveRequestService ?? new LeaveRequestService();
            _OvertimeService = overtimeService ?? new OvertimeRequestService();
        }
        #endregion

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        /// <summary>
        /// ActionResult Member.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View Member.</returns>
        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult Member(RequestViewModel request)
        {
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = true;
            ViewBag.Action = "Member";
            this.GetRequests(ref request, RequestViewType.PersonalSpace, false);

            return View(request);
        }

        /// <summary>
        ///  ActionResult HumanResourceView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View HumanResource.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel request)
        {
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Action = "HRView";
            this.GetRequests(ref request, RequestViewType.HrSpace, false);

            return View(request);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View Employee.</returns>
        public ActionResult EmployeeView(RequestViewModel request)
        {
            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Action = "EmployeeView";
            this.GetRequests(ref request, RequestViewType.CompanySpace, false);

            return View("HRView", request);
        }

        /// <summary>
        /// ActionResult DailyView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View DailyRequest.</returns>
        public ActionResult DailyView(RequestViewModel request)
        {
            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Action = "DailyView";
            this.GetRequests(ref request, RequestViewType.CompanySpace, true);

            return View("DailyRequestsView", request);
        }

        /// <summary>
        /// ActionResult HumanResourceViewHistory.
        /// </summary>
        /// <param name="id">CheckInOutRequestHistory id.</param>
        /// <param name="type">CheckInOutRequestHistory type.</param>
        /// <returns>View HumanResourceViewHistory.</returns>
        public ActionResult HRViewHistory(long id, int type)
        {
            switch (type)
            {
                case (int)RequestFilter.CheckInOut:
                    GetCheckInOutRequestHistory(id, type);
                    break;
                case (int)RequestFilter.LateInEarlyOut:
                    GetLateInEarlyOutRequestHistory(id, type);
                    break;
                case (int)RequestFilter.ChangeShift:
                    GetChangeShiftRequestHistory(id, type);
                    break;
                case (int)RequestFilter.Leave:
                    GetLeaveRequestHistory(id, type);
                    break;
                case (int)RequestFilter.KeepInView:
                    GeKeepInViewRequestHistory(id, type);
                    break;
            }

            ViewBag.requestId = id;
            ViewBag.requestType = type;

            return View();
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            return _RequestService.DeleteWorkingTimeRequest(id);
        }

        public JsonResult GetEmployeesByActingUser(int role, int requestType, string query)
        {
            var supervisors = _RequestService.GetSupervisorByQuery(this.CurrentUser.Id, role, requestType, query.Trim(), includeLoginUser: true);

            return Json(supervisors, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Member.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="type">The request type.</param>
        /// <returns>
        /// View Member.
        /// </returns>
        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult MemberDetail(int requestId, int type)
        {
            ViewBag.CurrentUserId = this.CurrentUser.Id;

            if (type == (int)RequestFilter.Leave)
            {
                return MemberLeaveDetail(requestId, "member");
            }
            else if (type == (int)RequestFilter.ChangeShift)
            {
                return MemberChangeShiftDetail(requestId, "member");
            }

            return RedirectToAction("member");
        }

        /// <summary>
        /// HRs the view detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="type">The request type.</param>
        /// <returns>View of HRViewDetail.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRViewDetail(int requestId, int type)
        {
            ViewBag.IsAllowChangeStatus = true;

            if (type == (int)RequestFilter.Leave)
            {
                return HRViewLeaveDetail(requestId, "hrview");
            }
            else if (type == (int)RequestFilter.ChangeShift)
            {
                return HRViewChangeShiftDetail(requestId, "hrview");
            }

            return RedirectToAction("hrview");
        }

        /// <summary>
        /// Employees view detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="type">The request type.</param>
        /// <returns>
        /// View of EmployeeViewDetail.
        /// </returns>
        public ActionResult EmployeeViewDetail(int requestId, int type)
        {
            ViewBag.IsAllowChangeStatus = false;

            if (type == (int)RequestFilter.Leave)
            {
                return HRViewLeaveDetail(requestId, "employeeview");
            }
            else if (type == (int)RequestFilter.ChangeShift)
            {
                return HRViewChangeShiftDetail(requestId, "employeeview");
            }

            return RedirectToAction("employeeview");
        }

        /// <summary>
        /// Employees view daily detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="type">The request type.</param>
        /// <returns>
        /// View of DailyViewDetail.
        /// </returns>
        public ActionResult DailyViewDetail(int requestId, int type)
        {
            ViewBag.IsAllowChangeStatus = false;

            if (type == (int)RequestFilter.Leave)
            {
                return HRViewLeaveDetail(requestId, "dailyview");
            }
            else if (type == (int)RequestFilter.ChangeShift)
            {
                return HRViewChangeShiftDetail(requestId, "dailyview");
            }

            return RedirectToAction("dailyview");
        }

        /// <summary>
        /// Check if user can and has permission to edit a request.
        /// </summary>
        /// <param name="status">Status of request.</param>
        /// <param name="employeeId">Which employee request belong to.</param>
        /// <param name="creatorId">The person who creates this request.</param>
        /// <returns>Return true if current user can edit this request.</returns>
        protected bool Editable(byte status, int employeeId, int creatorId)
        {
            if (status != (byte)CurrentRequestStatus.Pending ||
                (employeeId != this.CurrentUser.Id && creatorId != this.CurrentUser.Id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Function GetRequests.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <param name="requestViewType">Request View Types.</param>
        /// <param name="isDailyRequest">Is Daily Request.</param>
        [NonAction]
        private void GetRequests(ref RequestViewModel request, RequestViewType requestViewType, bool isDailyRequest)
        {
            request.TeamCollection = _TeamService.GetAll();
            request.ToDate = Utilities.AddTimeToDate(request.ToDate, 23, 59, 59);
            int? loginId = null;

            if (requestViewType != RequestViewType.HrSpace)
            {
                loginId = this.CurrentUser.Id;
            }

            var requestList = _RequestService.GetWorkingTimeRequests(request, requestViewType, isDailyRequest, loginId, request.IsLocal);
            ViewBag.CurrentUserId = this.CurrentUser.Id;
            ViewBag.Options = request;
            ViewBag.IsDailyRequest = isDailyRequest;
            ViewBag.EntityList = requestList;
        }

        [NonAction]
        private void GetLeaveRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _LeaveRequestService.GetHistoricLeaveRequests(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetChangeShiftRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _ChangeShiftRequestService.GetHistoricChangeShiftRequests(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetLateInEarlyOutRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _LateInEarlyOutRequestService.GetHistoricLateInEarlyOutRequests(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GetCheckInOutRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _CheckInOutRequestService.GetHistoricCheckInOutRequests(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        [NonAction]
        private void GeKeepInViewRequestHistory(long requestId, int requestType)
        {
            IList<RequestEntity> entityList = _KeepInViewRequestService.GetHistoricKeepInViewRequests(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, ChangeShiftViewModel viewModel)
        {
            var verifierList = _EmployeeService.GetListSupervisorName(id, (int)RolesEnum.Verifier, (int)RequestTypes.ChangeShift);
            var approverList = _EmployeeService.GetListSupervisorName(id, (int)RolesEnum.Approver, (int)RequestTypes.ChangeShift);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        /// <summary>
        /// HRs the view change shift detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>
        /// The view model of HRViewChangeShiftDetail.
        /// </returns>
        private ActionResult HRViewChangeShiftDetail(int requestId, string viewName)
        {
            var shift = _ChangeShiftRequestService.GetChangeShiftRequestById(requestId);

            if (shift == null)
            {
                return RedirectToAction(viewName);
            }

            var model = ViewChangeShiftDetail(requestId, viewName, shift);
            return View("HRViewDetail", model);
        }

        /// <summary>
        /// Members the change shift detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>
        /// The view model of HRViewChangeShiftDetail.
        /// </returns>
        private ActionResult MemberChangeShiftDetail(int requestId, string viewName)
        {
            var shift = _ChangeShiftRequestService.GetChangeShiftRequestById(requestId);

            if (shift == null)
            {
                return RedirectToAction(viewName);
            }

            var model = ViewChangeShiftDetail(requestId, viewName, shift);
            return View("MemberDetail", model);
        }

        /// <summary>
        /// Views the change shift detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>The model of ChangeShiftViewModel.</returns>
        private ChangeShiftViewModel ViewChangeShiftDetail(int requestId, string viewName, RequestEntity shift)
        {
            var listUnavailDates = _EmployeeService.GetUnavailableDates(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var shiftDetails = _ChangeShiftRequestService.GetShiftDetailsById(requestId);
            var shifts = _ShiftService.GetAll();
            var employee = _EmployeeService.GetEmployeeById(shift.EmpId);
            ViewBag.Shift = shifts;
            var model = new ChangeShiftViewModel();
            if (shift != null && shiftDetails != null)
            {
                model = this.ComposeHRViewChangeShiftDetailViewModel(shift, shiftDetails, employee, model);
            }

            GetVerifierApproverList(employee.Id, model);
            ViewBag.ViewName = viewName;

            return model;
        }

        /// <summary>
        /// HRs the view leave detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>
        /// The view model of HRViewLeaveDetail.
        /// </returns>
        private ActionResult HRViewLeaveDetail(int requestId, string viewName)
        {
            var leaveRequest = _LeaveRequestService.GetLeaveRequestById(requestId);

            if (leaveRequest == null)
            {
                return RedirectToAction(viewName);
            }

            ViewLeaveDetail(requestId, viewName, leaveRequest);
            return View("HRViewDetail", leaveRequest);
        }

        /// <summary>
        /// Members the leave detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>
        /// The view model of HRViewLeaveDetail.
        /// </returns>
        private ActionResult MemberLeaveDetail(int requestId, string viewName)
        {
            var leaveRequest = _LeaveRequestService.GetLeaveRequestById(requestId);

            if (leaveRequest == null)
            {
                return RedirectToAction(viewName);
            }

            ViewLeaveDetail(requestId, viewName, leaveRequest);
            return View("MemberDetail", leaveRequest);
        }

        /// <summary>
        /// Views the leave detail.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="leaveRequest">The leave request.</param>
        private void ViewLeaveDetail(int requestId, string viewName, LeaveRequestViewModel leaveRequest)
        {
            var currentUserId = leaveRequest.EmployeeId;
            var employee = _EmployeeService.GetEmployeeById(currentUserId);
            var employeeBalance = _LeaveRequestService.GetAnnualLeaveAndKeepInViewBalance(currentUserId, requestId, DateTime.Now.Date);
            var leaveTypes = _LeaveTypeService.GetAll();
            var verifierList = _EmployeeService.GetListSupervisorName(currentUserId, (int)RolesEnum.Verifier, (int)RequestTypes.Leave);
            var approverList = _EmployeeService.GetListSupervisorName(currentUserId, (int)RolesEnum.Approver, (int)RequestTypes.Leave);

            leaveRequest.LeaveTypeList = leaveTypes;
            leaveRequest.VerifierList = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            leaveRequest.ApproverList = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            leaveRequest.EmployeeBalance = employeeBalance;
            leaveRequest.RequestId = requestId;
            ViewBag.ViewName = viewName;
        }

        /// <summary>
        /// Composes the HR view change shift detail view model.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <param name="shiftDetails">The shift details.</param>
        /// <param name="employee">The employee.</param>
        /// <param name="model">The model.</param>
        /// <returns>The object of ChangeShiftViewModel.</returns>
        private ChangeShiftViewModel ComposeHRViewChangeShiftDetailViewModel(
            RequestEntity shift,
            List<ShiftDetails> shiftDetails,
            Employee employee,
            ChangeShiftViewModel model)
        {
            List<ShiftDetailsViewModel> lstShiftDeatails = new List<ShiftDetailsViewModel>();
            foreach (var item in shiftDetails)
            {
                ShiftDetailsViewModel shiftDetail = new ShiftDetailsViewModel
                {
                    Date = item.OnDate.ToString("MM/dd/yyyy"),
                    FromShift = item.FromShiftID,
                    ToShift = item.ToShiftID,
                    ScheduleStatus = item.FromShift,
                    ChangeToShiftCode = item.ToShift,
                    Remark = item.Remark,
                    ShiftId = item.ToShiftID,
                    HasData = true
                };
                lstShiftDeatails.Add(shiftDetail);
            }

            model = new ChangeShiftViewModel
            {
                RequestId = shift.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = shift.FromDate.Date,
                ToDate = shift.ToDate.Date,
                VerifierId = shift.VerifiedBy.Id,
                ApproverId = shift.ApprovedBy.Id,
                Remark = shift.Remark,
                ShiftDetails = lstShiftDeatails,
                Status = shift.Status.Id,
                FormattedStatus = shift.Status.Name
            };
            return model;
        }
    }
}
