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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;

    [Authorize]
    public class RequestController : BaseController
    {
        #region protected services

        private readonly IEmployeeService _employeeService;
        private readonly IShiftService _shiftService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly IKeepInViewRequestService _keepInViewRequestService;
        private readonly IChangeShiftRequestService _changeShiftRequestService;
        private readonly ICheckInOutRequestService _checkInOutRequestService;
        private readonly ILateInEarlyOutRequestService _lateInEarlyOutRequestService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IOvertimeRequestService _overtimeService;
        private readonly IWorkingCalendarService _workingCalendarService;

        #endregion protected services

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

        #endregion Ctor

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

        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult Member(RequestViewModel request)
        {
            request.ViewType = RequestViewType.PersonalSpace;

            if (request.StatusId == -2)
            {
                request.StatusId = -1;
                request.FromDate = DateTime.Now.AddDays(-60);
                request.IsFilterByAffectedDate = false;
            }

            GetRequests(request);
            return View("EmployeeRequest", request);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == -2 ? 0 : request.StatusId;
            request.ViewType = RequestViewType.HRSpace;
            GetRequests(request);

            return View("EmployeeRequest", request);
        }

        public ActionResult DailyView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == -2 ? 0 : request.StatusId;
            request.ViewType = RequestViewType.CompanySpace;
            GetRequests(request);

            return View("EmployeeRequest", request);
        }

        public ActionResult ViewHistory(long id, int type)
        {
            var criteria = new GetHistoryRequestCriteria()
            {
                RequestId = id,
                RequestType = type
            };
            IList<RequestEntity> requests = RequestService.GetHistoryRequest(criteria);
            ViewBag.EntityList = requests;
            ViewBag.requestType = type;

            return View();
        }

        [Authorize(Roles = "SystemAdmin")]
        public ActionResult VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == -2 ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            GetRequests(request);

            return View("EmployeeRequest", request);
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

        public JsonResult GetEmployeesByActingUser(int role, string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria()
            {
                EmpId = CurrentUser.Id,
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
            IList<SupervisorNameModel> verifierList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Verifier, requestType);

            return Json(verifierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> approverList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Approver, requestType);

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

        public PartialViewResult PartialViewProcessRequest(ProcessRequestViewModel model)
        {
            if (model != null && model.RequestId != 0 && model.RequestTypeId != 0 && model.RoleId != 0 && !string.IsNullOrEmpty(model.Command))
            {
                if (model.RoleId == (int)RoleSystem.Approver && model.Command == Command.Approve.ToString())
                {
                    switch (model.RequestTypeId)
                    {
                        case (int)RequestType.CheckIn:
                        case (int)RequestType.CheckOut:
                            var checkInOutDetailCriteria = new GetRequestByIdCriteria()
                            {
                                RequestId = model.RequestId
                            };
                            model.CheckInOutDetail = _requestService.GetRequestById(checkInOutDetailCriteria);
                            break;

                        case (int)RequestType.LateIn:
                        case (int)RequestType.EarlyOut:
                            var lateInEarlyOutDetailCriteria = new GetRequestByIdCriteria()
                            {
                                RequestId = model.RequestId
                            };
                            model.LateInEarlyOutDetail = _requestService.GetRequestById(lateInEarlyOutDetailCriteria);
                            break;

                        case (int)RequestType.ChangeShift:
                            ViewBag.Shifts = _shiftService.GetAll(isShowOff: true);
                            model.ChangeShiftDetails = GetChangeShiftDetail(model.RequestId);
                            break;

                        case (int)RequestType.Leave:
                            var criteria = new GetLeaveTypeCriteria()
                            {
                                ExcludeNotShown = true
                            };
                            ViewBag.LeaveTypes = _leaveTypeService.GetAll(criteria);
                            model.LeaveDetails = GetLeaveRequestDetail(model.RequestId);
                            break;
                    }
                }
            }

            return PartialView("_ProcessRequest", model);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessRequest(ProcessRequestViewModel model)
        {
            CommandResult result;
            if (model.Command == Command.Accept.ToString()
                || model.Command == Command.Approve.ToString()
                || model.Command == Command.Process.ToString())
            {
                result = ApproveRequest(model);
            }
            else
            {
                var criteria = new ProcessRequestCriteria()
                {
                    Action = model.Command,
                    RequestId = model.RequestId,
                    RoleId = model.RoleId,
                    Comment = model.Comment,
                    ProcessedBy = CurrentUser.Id
                };
                result = _requestService.Process(criteria);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected bool Editable(byte status, int employeeId, int creatorId)
        {
            return status == (byte)CurrentRequestStatus.Pending && (employeeId == CurrentUser.Id || creatorId == CurrentUser.Id);
        }

        protected string CheckRoleAccordingToRequestType(RequestViewType requestViewType)
        {
            string role;
            switch (requestViewType)
            {
                case RequestViewType.PersonalSpace:
                    role = ((int)RoleSystem.Verifier).ToString() + "," + ((int)RoleSystem.Approver).ToString();
                    break;

                case RequestViewType.HRSpace:
                    role = ((int)RoleSystem.HR).ToString();
                    break;

                default:
                    role = ((int)RoleSystem.Member).ToString();
                    break;
            }

            return role;
        }

        protected string GetStatus(int statusId)
        {
            string statusIds;
            switch (statusId)
            {
                case -1:
                    // status = -1 --> pending and verified (1,2)
                    statusIds = "1,2";
                    break;

                case 0:
                    statusIds = string.Empty;
                    break;

                default:
                    statusIds = statusId.ToString();
                    break;
            }

            return statusIds;
        }

        [NonAction]
        private void GetRequests(RequestViewModel request)
        {
            var getGroupCriteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };

            request.TeamCollection = _teamService.GetBySupervisor(getGroupCriteria);
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
                IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
                Role = CheckRoleAccordingToRequestType(request.ViewType),
                SupervisorId = CurrentUser.Id,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            request.CurrentUserId = CurrentUser.Id;
            request.Requests = requestList;
        }

        private IList<ChangeShiftDetailViewModel> GetChangeShiftDetail(long requestId)
        {
            var changeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria()
            {
                RequestId = requestId
            };
            IList<ChangeShiftDetailModel> changeShiftDetails = _changeShiftRequestService.GetChangeShiftRequestDetailById(changeShiftDetailCriteria);

            return ConvertChangeShiftDetailToViewModel(changeShiftDetails);
        }

        private IList<ChangeShiftDetailViewModel> ConvertChangeShiftDetailToViewModel(IList<ChangeShiftDetailModel> model)
        {
            return model.Select(item => new ChangeShiftDetailViewModel()
            {
                OnDate = item.OnDate,
                ScheduleStatus = item.FromShiftId == -1 ? Labels.Off : item.FromShift,
                FromShiftId = item.FromShiftId,
                ToShiftId = item.ToShiftId,
                Remark = item.Remark
            }).ToList();
        }

        private IList<LeaveDetailModel> GetLeaveRequestDetail(long requestId)
        {
            var leaveDetailCriteria = new GetLeaveRequestDetailCriteria()
            {
                RequestId = requestId
            };

            IList<LeaveDetailModel> leaveRequestDetail = _requestService.GetLeaveRequestDetail(leaveDetailCriteria);

            return leaveRequestDetail;
        }

        private CommandResult ApproveRequest(ProcessRequestViewModel model)
        {
            var criteria = new ApproveRequestCriteria()
            {
                RequestId = model.RequestId,
                RoleId = model.RoleId,
                Comment = model.Comment,
                IgnoredDisciplines = model.IgnoredDisciplines,
                ProcessedBy = CurrentUser.Id
            };
            CommandResult result = _requestService.ApproveRequest(criteria);

            return result;
        }
    }
}