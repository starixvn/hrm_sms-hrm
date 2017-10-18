namespace Fanex.NexHR.Controllers.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class RequestController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IShiftService shiftService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public RequestController()
            : this(null, null, null, null, null)
        {
        }

        public RequestController(
            IEmployeeService employeeService,
            IShiftService shiftService,
            IRequestService requestService,
            ITeamService teamService,
            ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.shiftService = shiftService ?? new ShiftService();
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.commandService = commandService ?? new CommandService();
        }

        [HttpPost]
        public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            var date = RequestService.ToRequestMessage(request);
            return date;
        }

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        [HRAuthorize(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
        public ActionResult Member(RequestViewModel request)
        {
            request.ViewType = RequestViewType.PersonalSpace;

            if (request.StatusId == -2)
            {
                request.StatusId = -1;
                request.FromDate = DateTime.Now.Date.AddDays(-60);
                request.IsFilterByAffectedDate = false;
            }

            GetRequests(request);
            return View("EmployeeRequest", request);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ProcessRequest)]
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

        [HRAuthorize(Policy.Host, FunctionKey.VoidRequest)]
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

            requestService.SendMailForDeleteSuccessfully(id);
            CommandResult result = commandService.Delete(criteria);
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

        [HRAuthorize(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
        public PartialViewResult PartialViewProcessRequest(ProcessRequestViewModel model)
        {
            if (model != null
                && model.RequestId != 0
                && model.RoleId == (int)RoleSystem.Approver
                && model.Command == Command.Approve.ToString())
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
                        model.LeaveDetails = GetLeaveRequestDetailToIgnoreById(model.RequestId);
                        break;
                }
            }

            return PartialView("_ProcessRequest", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
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
                result = Process(model);
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
            request.TeamCollection = GetGroupBySupervisor();
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
            request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(requestList) : requestList;
        }

        private IList<RequestEntity> SortRequest(IList<RequestEntity> requests)
        {
            return requests.OrderByDescending(t => (t.Status.Id == (byte)CurrentRequestStatus.Pending
                && (t.ApprovedBy.Id == CurrentUser.Id || t.VerifiedBy.Id == CurrentUser.Id))
                || (t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApprovedBy.Id == CurrentUser.Id)).ToList();
        }

        private IList<Team> GetGroupBySupervisor()
        {
            var criteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };

            IList<Team> teams = teamService.GetBySupervisor(criteria);
            return teams;
        }

        private List<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(long requestId)
        {
            var criteria = new GetChangeShiftDetailToIgnoreByIdCriteria()
            {
                RequestId = requestId
            };
            IList<ChangeShiftDetailModel> changeShiftDetails = requestService.GetChangeShiftDetailToIgnoreById(criteria);

            return changeShiftDetails.ToList();
        }

        private List<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(long requestId)
        {
            var leaveDetailCriteria = new GetLeaveRequestDetailToIgnoreByIdCriteria()
            {
                RequestId = requestId
            };

            IList<LeaveDetailModel> leaveRequestDetail = requestService.GetLeaveRequestDetailToIgnoreById(leaveDetailCriteria);

            return leaveRequestDetail.ToList();
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

            CommandResult result = commandService.Update(criteria);
            if (result != null && result.IsSuccessful)
            {
                requestService.SendMailForProcessSuccessfully(criteria.RequestId, criteria.ProcessedBy, model.Command);
            }

            return result;
        }

        private CommandResult Process(ProcessRequestViewModel model)
        {
            var criteria = new ProcessRequestCriteria()
            {
                Action = model.Command,
                RequestId = model.RequestId,
                RoleId = model.RoleId,
                Comment = model.Comment,
                ProcessedBy = CurrentUser.Id
            };

            CommandResult result = commandService.Update(criteria);
            if (result != null && result.IsSuccessful)
            {
                requestService.SendMailForProcessSuccessfully(criteria.RequestId, criteria.ProcessedBy, criteria.Action);
            }

            return result;
        }

        public ActionResult TestPage()
        {
            return View("TestPage");
        }
    }
}