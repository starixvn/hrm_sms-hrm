namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Dashboard;
    using Fanex.NexHR.Services;

    public class HomeController : BaseController
    {
        private readonly IDashboardService dashboardService;
        private readonly IEmployeeService employeeService;

        public HomeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IDashboardService dashboardService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.dashboardService = dashboardService;
        }

        public ActionResult Index()
        {
            IEnumerable<SummaryRequest> model = dashboardService.GetMySpace(sessionService.Employee.Id, DateTime.Now);
            ViewBag.IsResetPassword = employeeService.CheckResetPassword(sessionService.Employee.Id);
            ViewBag.IsVerifierApprover = sessionService.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || sessionService.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult MemberRequest()
        {
            IEnumerable<MemberRequest> model = dashboardService.GetMemberRequest(sessionService.Employee.Id, DateTime.Now);

            return PartialView("_MemberRequest", model);
        }
    }
}
