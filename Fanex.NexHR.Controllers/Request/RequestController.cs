namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class RequestController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IShiftService shiftService;
        private readonly IRequestService requestService;
        private readonly IRequestMailService requestMailService;
        private readonly ICommonDataService commonDataService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public RequestController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IShiftService shiftService,
            IRequestService requestService,
            IRequestMailService requestMailService,
            ICommonDataService commonDataService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.shiftService = shiftService;
            this.requestService = requestService;
            this.requestMailService = requestMailService;
            this.teamService = teamService;
            this.baseService = baseService;
            this.commonDataService = commonDataService;
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

        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        public ActionResult Member(RequestViewModel request)
        {
            request.ViewType = RequestViewType.PersonalSpace;

            if (request.StatusId == -2)
            {
                request.StatusId = Constants.PendingAndVerifiedStatusId;
                request.FromDate = DateTime.Now.Date.AddDays(-60);
                request.IsFilterByAffectedDate = false;
            }
            GetRequests(request);

            return View("EmployeeRequest", request);
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
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
            var criteria = new GetHistoryRequestCriteria
            {
                RequestId = id,
                RequestType = type
            };
            IList<RequestEntity> requests = requestService.GetHistoryRequest(criteria);
            ViewBag.EntityList = requests;
            ViewBag.requestType = type;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public ActionResult VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == -2 ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            GetRequests(request);

            return View("EmployeeRequest", request);
        }

        public JsonResult GetEmployeesByActingUser(byte role, string query = "", byte? requestType = null)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = sessionService.Employee.Id,
                Role = role,
                RequestType = requestType,
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

        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public PartialViewResult PartialViewProcessRequest(ProcessRequestViewModel model)
        {
            if (model != null
                && model.RequestId != 0
                && model.RoleId == (int)RoleSystem.Approver
                && model.Command == RequestCommand.Approve.ToString())
            {
                HandleProcessRequest(model);
            }

            return PartialView("_ProcessRequest", model);
        }

        private void HandleProcessRequest(ProcessRequestViewModel model)
        {
            switch (model.RequestTypeId)
            {
                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    var getCheckInOutCriteria = new GetCheckInOutRequestToIgnoreByIdCriteria
                    {
                        RequestId = model.RequestId
                    };
                    model.CheckInOutDetail = requestService.GetCheckInOutRequestToIgnoreById(getCheckInOutCriteria);
                    break;

                case (int)RequestType.LateIn:
                case (int)RequestType.EarlyOut:
                    var getLateInEarlyOutCriteria = new GetLateInEarlyOutRequestToIgnoreByIdCriteria
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
                    model.LeaveDisciplines = GetLeaveRequestDetailToIgnoreById(model.RequestId);
                    break;

                default:
                    return;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public JsonResult ProcessRequest(ProcessRequestViewModel model)
        {
            CommandResult result;

            result = (model.Command == RequestCommand.Accept.ToString()
                || model.Command == RequestCommand.Approve.ToString()
                || model.Command == RequestCommand.Process.ToString())
                ? ApproveRequest(model) : Process(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private void GetRequests(RequestViewModel request)
        {
            request.TeamCollection = teamService.GetBySupervisor(sessionService.Employee.Id);
            request.ToDate = Utilities.SetTimeToEndOfDay(request.ToDate);
            request.LeaveTypeCollection = commonDataService.GetLeaveType();
            int? employeeId = null;
            if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
            {
                employeeId = request.EmployeeId;
            }

            bool? isLocal = null;
            var criteria = new GetRequestCriteria
            {
                EmpId = employeeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                GroupIds = request.TeamIds,
                Status = request.StatusIds,
                RequestType = request.RequestType,
                IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
                RequestViewType = request.ViewType,
                SupervisorId = sessionService.Employee.Id,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal,
                LeaveTypeId = request.LeaveType
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            request.CurrentUserId = sessionService.Employee.Id;
            request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(requestList) : requestList;
        }

        private IList<RequestEntity> SortRequest(IList<RequestEntity> requests)
        {
            return requests.OrderByDescending(t => (SortRequestValidate(t))).ToList();
        }

        private bool SortRequestValidate(RequestEntity t)
        {
            bool pending = t.Status.Id == (byte)CurrentRequestStatus.Pending
                && (t.ApprovedBy.Id == sessionService.Employee.Id || t.VerifiedBy.Id == sessionService.Employee.Id);
            bool verified = (t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApprovedBy.Id == sessionService.Employee.Id);
            return pending || verified;
        }

        private IList<ChangeShiftDetail> GetChangeShiftDetailToIgnoreById(long requestId)
        {
            var criteria = new GetChangeShiftDetailToIgnoreByIdCriteria
            {
                RequestId = requestId
            };
            IList<ChangeShiftDetail> changeShiftDetails = requestService.GetChangeShiftDetailToIgnoreById(criteria);

            return changeShiftDetails.ToList();
        }

        private IList<LeaveDisciplineModel> GetLeaveRequestDetailToIgnoreById(long requestId)
        {
            var leaveDetailCriteria = new GetLeaveRequestDetailToIgnoreByIdCriteria
            {
                RequestId = requestId
            };

            IList<LeaveDisciplineModel> disciplines = requestService.GetLeaveRequestDetailToIgnoreById(leaveDetailCriteria);

            return disciplines;
        }

        private CommandResult ApproveRequest(ProcessRequestViewModel model)
        {
            var criteria = new ApproveRequestCriteria
            {
                RequestId = model.RequestId,
                RoleId = model.RoleId,
                Comment = model.Comment,
                IgnoredDisciplines = model.IgnoredDisciplines,
                ProcessedBy = sessionService.Employee.Id
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                requestMailService.SendMailForProcessSuccessfully(criteria.RequestId, criteria.ProcessedBy, model.Command);
            }

            return result;
        }

        private CommandResult Process(ProcessRequestViewModel model)
        {
            var criteria = new ProcessRequestCriteria
            {
                Action = model.Command,
                RequestId = model.RequestId,
                RoleId = model.RoleId,
                Comment = model.Comment,
                ProcessedBy = sessionService.Employee.Id
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                requestMailService.SendMailForProcessSuccessfully(criteria.RequestId, criteria.ProcessedBy, criteria.Action);
            }

            return result;
        }

        public ActionResult ViewHistoryNewVersion(long id, int type)
        {
            var criteria = new GetHistoryRequestCriteria
            {
                RequestId = id,
                RequestType = type
            };
            IList<RequestEntity> requests = requestService.GetHistoryRequest(criteria);
            ViewBag.EntityList = requests;
            ViewBag.requestType = type;

            return View("ViewHistoryNewVersion");
        }
    }
}