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
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Overtime;
    using Hangfire;

    public class OvertimeController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;

        public OvertimeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IRequestService requestService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.requestService = requestService;
            this.teamService = teamService;
            this.overtimeService = overtimeService;
        }

        [HttpGet]
        public ActionResult EmployeeView(MonthlyOvertimeRequestViewModel model)
        {
            model.Employee = model.Employee ?? sessionService.Employee.Nickname;
            model.EmployeeId = model.EmployeeId <= 0 ? sessionService.Employee.Id : model.EmployeeId;
            model.OvertimeRequests = overtimeService.GetMonthlyRequestByEmployeeId(model.EmployeeId, model.Year);

            return View(model);
        }

        public ActionResult Details(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);

            return View(monthlyRequest);
        }

        public ActionResult Edit(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.Overtime);

            var model = new EditMonthlyRequestViewModel
            {
                MonthlyRequest = monthlyRequest,
                VerifierId = monthlyRequest.Verifier != null ? monthlyRequest.Verifier.EmployeeId : 0,
                ApproverId = monthlyRequest.Approver != null ? monthlyRequest.Approver.EmployeeId : 0,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
            };

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Edit(EditMonthlyOvertimeRequest model)
        {
            CommandResult commandResult = overtimeService.EditMonthlyRequest(sessionService.Employee.Id, model);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var statusIds = string.Empty;
            IEnumerable<int> requestStatusIds = Enum.GetValues(typeof(CurrentRequestStatus)).Cast<int>().Where(x => x > 0);
            IEnumerable<Team> sections = teamService.GetAll();
            if (model.StatusId == -1)
            {
                statusIds = string.Format("{0},{1}", (byte)CurrentRequestStatus.Pending, (byte)CurrentRequestStatus.Verified);
            }
            else if (model.StatusId == 0)
            {
                statusIds = string.Join(",", requestStatusIds);
            }
            else
            {
                statusIds = model.StatusId.ToString();
            }
            var param = new GetMonthlyOvertimeRequestParam
            {
                ActorId = sessionService.Employee.Id,
                Month = model.Month,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                StatusIds = statusIds
            };
            model.Sections = sections.Select(x => new EntityBase<int> { Id = x.Id, Name = x.Name, Code = x.Code });
            model.MemberRequests = overtimeService.GetMemberMonthlyRequest(param);

            ViewBag.CurrentUserId = sessionService.Employee.Id;
            ViewBag.IsVerifier = sessionService.HasPermissionOnFunction(FunctionKey.VerifyOvertime, Policy.Host);
            ViewBag.IsApprover = sessionService.HasPermissionOnFunction(FunctionKey.ApproveOvertime, Policy.Host);

            return View(model);
        }

        public ActionResult MemberDetails(long id)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsById(id);
            ViewBag.CurrentUserId = sessionService.Employee.Id;
            return View(monthlyRequest);
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult EmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var statusIds = string.Empty;
            IEnumerable<int> requestStatusIds = Enum.GetValues(typeof(CurrentRequestStatus)).Cast<int>().Where(x => x > 0);
            IEnumerable<Team> sections = teamService.GetAll();
            if (model.StatusId == -1)
            {
                statusIds = string.Format("{0},{1}", (byte)CurrentRequestStatus.Pending, (byte)CurrentRequestStatus.Verified);
            }
            else if (model.StatusId == 0)
            {
                statusIds = string.Join(",", requestStatusIds);
            }
            else
            {
                statusIds = model.StatusId.ToString();
            }
            var param = new GetMonthlyOvertimeRequestParam
            {
                ActorId = sessionService.Employee.Id,
                Month = model.Month,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                StatusIds = statusIds,
                IsProcessor = true
            };
            model.Sections = sections.Select(x => new EntityBase<int> { Id = x.Id, Name = x.Name, Code = x.Code });
            model.MemberRequests = overtimeService.GetMemberMonthlyRequest(param);

            return View(model);
        }

        public ActionResult EmployeeDetails(long id)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsById(id);
            return View(monthlyRequest);
        }

        public ActionResult GetActingEmployees(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = sessionService.Employee.Id,
                Role = (int)RoleSystem.OnBehalfRequest,
                RequestType = (byte)RequestType.Overtime,
                Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
                IncludeLoginUser = true
            };

            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public JsonResult EditMonthlyRequestStatus(string requestIds, string command, byte roleId)
        {
            CommandResult commandResult = overtimeService.EditMonthlyRequestStatus(sessionService.Employee.Id, requestIds, command, roleId);
            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult CheckInvalidOvertime(string requestIds)
        {
            bool isInvalid = overtimeService.CheckInvalidOvertimes(requestIds);

            return Json(isInvalid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public void ReminderClaimOvertime(string subject, string content)
        {
            BackgroundJob.Enqueue(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
        }
    }
}