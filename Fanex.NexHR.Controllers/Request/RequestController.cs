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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
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
        #region protected services
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
        protected readonly IWorkingCalendarService _WorkingCalendarService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null, null, null, null, null, null, null, null, null, null)
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
            IOvertimeRequestService overtimeService = null,
            IWorkingCalendarService workingCalendarService = null)
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
            _WorkingCalendarService = workingCalendarService ?? new WorkingCalendarService();
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
            request.IsMember = true;
            
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = true;
            ViewBag.Action = "Member";

            this.GetRequests(request, RequestViewType.PersonalSpace, request.IsDailyRequest);
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
            if (request.StatusId == -1)
            {
                request.StatusId = 0;
                request.IsDailyRequest = true;
                request.FromDate = request.ToDate;
            }

            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Action = "HRView";
            this.GetRequests(request, RequestViewType.HrSpace, request.IsDailyRequest);

            return View(request);
        }

        /// <summary>
        /// ActionResult DailyView.
        /// </summary>
        /// <param name="request">Request ViewModel.</param>
        /// <returns>View DailyRequest.</returns>
        public ActionResult DailyView(RequestViewModel request)
        {
            if (request.StatusId == -1)
            {
                request.StatusId = 0;
                request.IsDailyRequest = true;
                request.FromDate = request.ToDate;
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = this.CurrentUser.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            request.IsMember = employee.IsMember();
            if (request.IsMember)
            {
                request.Team = employee.Team.Name;
                request.TeamIds = employee.Team.Id.ToString();
            }

            this.GetRequests(request, RequestViewType.CompanySpace, request.IsDailyRequest);
            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Action = "DailyView";

            return View("DailyRequestsView", request);
        }

        /// <summary>
        /// ActionResult View History.
        /// </summary>
        /// <param name="id">RequestHistory id.</param>
        /// <param name="type">Request type.</param>
        /// <returns>View HumanResourceViewHistory.</returns>
        public ActionResult HRViewHistory(long id, int type)
        {
            IList<RequestEntity> entityList = _RequestService.GetHistoryRequest(id, type);

            ViewBag.requestId = id;
            ViewBag.requestType = type;
            ViewBag.EntityList = entityList;
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

            if (type == (int)RequestFilter.ChangeShift)
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

            if (type == (int)RequestFilter.ChangeShift)
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

            if (type == (int)RequestFilter.ChangeShift)
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

            if (type == (int)RequestFilter.ChangeShift)
            {
                return HRViewChangeShiftDetail(requestId, "dailyview");
            }

            return RedirectToAction("dailyview");
        }

        public ActionResult GetVerifiers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> verifierList = this.GetSupervisorByRequestType(employeeId, (byte)RolesEnum.Verifier, requestType);

            return Json(verifierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> approverList = this.GetSupervisorByRequestType(employeeId, (byte)RolesEnum.Approver, requestType);

            return Json(approverList, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public IList<SupervisorNameModel> GetSupervisorByRequestType(int employeeId, byte role, byte requestType)
        {
            var criteria = new GetSupervisorByRequestTypeCriteria
            {
                EmpId = employeeId,
                Role = role,
                RequestType = requestType
            };
            IList<SupervisorNameModel> supervisors = _EmployeeService.GetSupervisorByRequestType(criteria);
            return supervisors;
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
        private void GetRequests(RequestViewModel request, RequestViewType requestViewType, bool isDailyRequest)
        {
            request.TeamCollection = _TeamService.GetAll();
            request.ToDate = Utilities.AddTimeToDate(request.ToDate, 23, 59, 59);
            int? loginId = null;
            
            if (requestViewType != RequestViewType.HrSpace)
            {
                loginId = this.CurrentUser.Id;
            }

            var requestList = _RequestService.GetRequests(
                requestViewType,
                request.RequestType,
                request.FromDate,
                request.ToDate,
                request.TeamIds,
                request.StatusId,
                isDailyRequest,
                request.EmployeeId,
                loginId,
                request.IsLocal);
            ViewBag.CurrentUserId = this.CurrentUser.Id;
            ViewBag.Options = request;
            ViewBag.IsDailyRequest = isDailyRequest;
            ViewBag.EntityList = requestList;
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, ChangeShiftViewModel viewModel)
        {
            var getListSupervisor = new GetListSupervisorNameCriteria()
            {
                EmpId = id,
                Role = null,
                RequestType = (int)RequestTypes.ChangeShift,
            };

            IList<SupervisorNameModel> supervisors = _EmployeeService.GetListSupervisorName(getListSupervisor);
            var verifierList = supervisors.Where(x => x.ActionType == (int)RolesEnum.Verifier);
            var approverList = supervisors.Where(x => x.ActionType == (int)RolesEnum.Approver);
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
        private ActionResult HRViewChangeShiftDetail(long requestId, string viewName)
        {
            var changeShift = _ChangeShiftRequestService.GetRequestById(requestId);

            if (changeShift == null)
            {
                return RedirectToAction(viewName);
            }

            var model = ViewChangeShiftDetail(requestId, viewName, changeShift);
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
            var shift = _ChangeShiftRequestService.GetRequestById(requestId);

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
        private ChangeShiftViewModel ViewChangeShiftDetail(long requestId, string viewName, RequestEntity shift)
        {
            var listUnavailDates = _EmployeeService.GetUnavailableDates(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var shiftDetails = _ChangeShiftRequestService.GetChangeShiftDetailsById(requestId);
            var shifts = _ShiftService.GetAll(isShowOff: true);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = shift.Requester.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
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
                    ScheduleStatus = item.FromShiftID == -1 ? Labels.Off : item.FromShift,
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
