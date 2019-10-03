namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class RequestController : BaseController
    {
        private const string EmployeeRequestViewName = "EmployeeRequest";
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly ICommonDataService commonDataService;
        private readonly ITeamService teamService;

        public RequestController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IRequestService requestService,
            ICommonDataService commonDataService,
            ITeamService teamService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.requestService = requestService;
            this.teamService = teamService;
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

        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        public ActionResult Member(RequestViewModel request)
        {
            request.ViewType = RequestViewType.PersonalSpace;

            if (request.StatusId == Constants.DefaultRequestFilterStatusId)
            {
                request.StatusId = Constants.PendingAndVerifiedStatusId;
                request.FromDate = DateTime.Now.Date.AddDays(-60);
                request.IsFilterByAffectedDate = false;
            }
            GetRequests(request);

            return View(EmployeeRequestViewName, request);
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        public ActionResult HRView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.HRSpace;
            GetRequests(request);

            return View(EmployeeRequestViewName, request);
        }

        public ActionResult DailyView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.CompanySpace;
            GetRequests(request);

            return View(EmployeeRequestViewName, request);
        }

        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public ActionResult VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            GetRequests(request);

            return View(EmployeeRequestViewName, request);
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
                && model.RequestId > 0
                && model.RoleId == (byte)RoleSystem.Approver
                && model.Command == RequestCommand.Approve)
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
                    model.CheckInOutDetail = requestService.GetCheckInOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (int)RequestType.LateIn:
                case (int)RequestType.EarlyOut:
                    model.LateInEarlyOutDetail = requestService.GetLateInEarlyOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (int)RequestType.ChangeShift:
                    model.ChangeShiftDetails = requestService.GetChangeShiftDetailToReviewDiscipline(model.RequestId);
                    break;

                case (int)RequestType.Leave:
                    model.LeaveDisciplines = requestService.GetLeaveRequestDetailToIgnoreById(model.RequestId);
                    break;

                default:
                    // nothing
                    break;
            }
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
    }
}