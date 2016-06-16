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
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

    public class RequestController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IShiftService shiftService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly ILeaveTypeService leaveTypeService;

        public RequestController()
            : this(null, null, null, null, null)
        {
        }

        public RequestController(
            IEmployeeService employeeService,
            IShiftService shiftService,
            IRequestService requestService,
            ITeamService teamService,
            ILeaveTypeService leaveTypeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.shiftService = shiftService ?? new ShiftService();
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.leaveTypeService = leaveTypeService ?? new LeaveTypeService();
        }

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        [Authorize(Roles = "Verifier, Approver")]
        [HRAuthorize(FunctionKey = FunctionKey.RequestOfMember, Policy = Policy.Read)]
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
        [HRAuthorize(FunctionKey = FunctionKey.RequestOfEmployee, Policy = Policy.Read)]
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
            IList<RequestEntity> requests = requestService.GetHistoryRequest(criteria);
            ViewBag.EntityList = requests;
            ViewBag.requestType = type;

            return View();
        }

        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.VoidRequest, Policy = Policy.Read)]
        public ActionResult VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == -2 ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            GetRequests(request);

            return View("EmployeeRequest", request);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteRequestCriteria()
            {
                RequestId = id
            };

            CommandResult result = requestService.Delete(criteria);
            return Json(result);
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
            IList<SupervisorNameModel> supervisors = employeeService.GetSupervisorByQuery(criteria);

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
            IList<SupervisorNameModel> supervisors = employeeService.GetSupervisorByRequestType(criteria);

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
                            var getCheckInOutCriteria = new GetCheckInOutRequestToIgnoreByIdCriteria()
                            {
                                RequestId = model.RequestId
                            };
                            model.CheckInOutDetail = requestService.GetCheckInOutRequestToIgnoreById(getCheckInOutCriteria);
                            break;

                        case (int)RequestType.LateIn:
                        case (int)RequestType.EarlyOut:
                            var getLateInEarlyOutCriteria = new GetLateInEarlyOutRequestToIgnoreByIdCriteria()
                            {
                                RequestId = model.RequestId
                            };
                            model.LateInEarlyOutDetail = requestService.GetlateInEarlyOutRequestToIgnoreById(getLateInEarlyOutCriteria);
                            break;

                        case (int)RequestType.ChangeShift:
                            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
                            model.ChangeShiftDetails = GetChangeShiftDetailToIgnoreById(model.RequestId);
                            break;

                        case (int)RequestType.Leave:
                            var criteria = new GetLeaveTypeCriteria()
                            {
                                ExcludeNotShown = true
                            };
                            ViewBag.LeaveTypes = leaveTypeService.GetAll(criteria);
                            model.LeaveDetails = GetLeaveRequestDetailToIgnoreById(model.RequestId);
                            break;
                    }
                }
            }

            return PartialView("_ProcessRequest", model);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(FunctionKey = FunctionKey.ProcessRequest, Policy = Policy.Edit)]
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
                result = requestService.Process(criteria);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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

            request.TeamCollection = teamService.GetBySupervisor(getGroupCriteria);
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
                RequestViewType = request.ViewType,
                SupervisorId = CurrentUser.Id,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            request.CurrentUserId = CurrentUser.Id;
            request.Requests = requestList;
        }

        private IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(long requestId)
        {
            var criteria = new GetChangeShiftDetailToIgnoreByIdCriteria()
            {
                RequestId = requestId
            };
            IList<ChangeShiftDetailModel> changeShiftDetails = requestService.GetChangeShiftDetailToIgnoreById(criteria);

            return changeShiftDetails;
        }

        private IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(long requestId)
        {
            var leaveDetailCriteria = new GetLeaveRequestDetailToIgnoreByIdCriteria()
            {
                RequestId = requestId
            };

            IList<LeaveDetailModel> leaveRequestDetail = requestService.GetLeaveRequestDetailToIgnoreById(leaveDetailCriteria);

            return leaveRequestDetail;
        }

        private CommandResult ApproveRequest(ProcessRequestViewModel model)
        {
            CommandResult result = new CommandResult();

            if (model.IgnoredDisciplines != null && model.IgnoredDisciplines.Any(x => x.IsIgnored == false && x.IsConfirmed == false))
            {
                result.Code = 1;
                result.Message = Messages.ProcessRequestWarning;
            }
            else
            {
                var criteria = new ApproveRequestCriteria()
                {
                    RequestId = model.RequestId,
                    RoleId = model.RoleId,
                    Comment = model.Comment,
                    IgnoredDisciplines = model.IgnoredDisciplines,
                    ProcessedBy = CurrentUser.Id
                };

                result = requestService.ApproveRequest(criteria);
            }

            return result;
        }
    }
}