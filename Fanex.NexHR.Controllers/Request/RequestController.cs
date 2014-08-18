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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;

    /// <summary>
    /// Request Controller.
    /// </summary>
    [Authorize]
    public class RequestController : BaseController
    {
        #region protected services
        private IEmployeeService _employeeService;
        private IShiftService _shiftService;
        private ILeaveTypeService _leaveTypeService;
        private IRequestService _requestService;
        private ITeamService _teamService;
        private IKeepInViewRequestService _keepInViewRequestService;
        private IChangeShiftRequestService _changeShiftRequestService;
        private ICheckInOutRequestService _checkInOutRequestService;
        private ILateInEarlyOutRequestService _lateInEarlyOutRequestService;
        private ILeaveRequestService _leaveRequestService;
        private IOvertimeRequestService _overtimeService;
        private IWorkingCalendarService _workingCalendarService;
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
            _employeeService = employeeService ?? new EmployeeService();
            _shiftService = shiftService ?? new ShiftService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _requestService = requestService ?? new RequestService();
            _teamService = teamService ?? new TeamService();
            _keepInViewRequestService = keepInViewRequestService ?? new KeepInViewRequestService();
            _changeShiftRequestService = changeShiftRequestService ?? new ChangeShiftRequestService();
            _checkInOutRequestService = checkInOutRequestService ?? new CheckInOutRequestService();
            _lateInEarlyOutRequestService = lateInEarlyOutRequestService ?? new LateInEarlyOutRequestService();
            _leaveRequestService = leaveRequestService ?? new LeaveRequestService();
            _overtimeService = overtimeService ?? new OvertimeRequestService();
            _workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
        }
        #endregion

        public IEmployeeService EmployeeService
        {
            get
            {
                return this._employeeService;
            }
        }

        public IShiftService ShiftService
        {
            get
            {
                return this._shiftService;
            }
        }

        public ILeaveTypeService LeaveTypeService
        {
            get
            {
                return this._leaveTypeService;
            }
        }

        public IRequestService RequestService
        {
            get
            {
                return this._requestService;
            }
        }

        public ITeamService TeamService
        {
            get
            {
                return this._teamService;
            }
        }

        public IKeepInViewRequestService KeepInViewRequestService
        {
            get
            {
                return this._keepInViewRequestService;
            }
        }

        public IChangeShiftRequestService ChangeShiftRequestService
        {
            get
            {
                return this._changeShiftRequestService;
            }
        }

        public ICheckInOutRequestService CheckInOutRequestService
        {
            get
            {
                return this._checkInOutRequestService;
            }
        }

        public ILateInEarlyOutRequestService LateInEarlyOutRequestService
        {
            get
            {
                return this._lateInEarlyOutRequestService;
            }
        }

        public ILeaveRequestService LeaveRequestService
        {
            get
            {
                return this._leaveRequestService;
            }
        }

        public IOvertimeRequestService OvertimeService
        {
            get
            {
                return this._overtimeService;
            }
        }

        public IWorkingCalendarService WorkingCalendarService
        {
            get
            {
                return this._workingCalendarService;
            }
        }

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
            this.GetRequests(request, RequestViewType.HRSpace, request.IsDailyRequest);

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
            var employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
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
            var criteria = new GetHistoryRequestCriteria()
            {
                RequestId = id,
                RequestType = type
            };
            IList<RequestEntity> entityList = RequestService.GetHistoryRequest(criteria);
            ViewBag.EntityList = entityList;
            ViewBag.requestType = type;
            return View();
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            var criteria = new DeleteRequestCriteria()
            {
                RequestId = id
            };

            CommandResult result = _requestService.Delete(criteria);
            return result.Code == 0;
        }

        /// <summary>
        /// Function to get employee by acting supervisor.
        /// </summary>
        /// <param name="role">Acting role.</param>
        /// <param name="query">The query.</param>
        /// <returns>List employees.</returns>
        public JsonResult GetEmployeesByActingUser(int role, string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria()
            {
                EmpId = this.CurrentUser.Id,
                Role = (byte)role,
                RequestType = null,
                Query = query.Trim(),
                IncludeLoginUser = true
            };

            IList<SupervisorNameModel> supervisors = EmployeeService.GetSupervisorByQuery(criteria);
            return Json(supervisors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVerifiers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> verifierList = this.GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Verifier, requestType);

            return Json(verifierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> approverList = this.GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Approver, requestType);

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
            IList<SupervisorNameModel> supervisors = _employeeService.GetSupervisorByRequestType(criteria);
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
        /// Method check Role.
        /// </summary>
        /// <param name="requestViewType">RequestViewType model.</param>
        /// <returns>Role after check.</returns>
        protected string CheckRoleAccordingToRequestType(RequestViewType requestViewType)
        {
            var role = string.Empty;
            if (requestViewType == RequestViewType.PersonalSpace)
            {
                role = ((int)RoleSystem.Verifier).ToString() + "," + ((int)RoleSystem.Approver).ToString();
            }
            else if (requestViewType == RequestViewType.HRSpace)
            {
                role = ((int)RoleSystem.HR).ToString();
            }
            else
            {
                role = ((int)RoleSystem.Member).ToString();
            }

            return role;
        }

        /// <summary>
        /// Function to get statusIds.
        /// </summary>
        /// <param name="statusId">Status Id from UI.</param>
        /// <returns>Status Ids.</returns>
        protected string GetStatus(int statusId)
        {
            string statusIds;
            if (statusId == -1)
            {
                // status = -1 --> pending and verified (1,2)
                statusIds = "1,2";
            }
            else if (statusId == 0)
            {
                statusIds = string.Empty;
            }
            else
            {
                statusIds = statusId.ToString();
            }

            return statusIds;
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
            var getAllGroupCriteria = new GetAllGroupCriteria();
            request.TeamCollection = _teamService.GetAll(getAllGroupCriteria);
            request.ToDate = Utilities.AddTimeToDate(request.ToDate, 23, 59, 59);
            int? employeeId = null;
            if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
            {
                employeeId = request.EmployeeId;
            }

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = employeeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                GroupIds = request.TeamIds,
                Status = GetStatus(request.StatusId),
                RequestType = request.RequestType,
                IsQueryByCreatedDate = !isDailyRequest,
                Role = CheckRoleAccordingToRequestType(requestViewType),
                SupervisorId = this.CurrentUser.Id,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
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
                RequestType = (int)RequestType.ChangeShift,
            };

            IList<SupervisorNameModel> supervisors = _employeeService.GetListSupervisorName(getListSupervisor);
            var verifierList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Verifier);
            var approverList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Approver);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }
    }
}
