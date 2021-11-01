namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Constants;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Dashboard;
    using Fanex.NexHR.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class HomeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDashboardService dashboardService;
        private readonly IEmployeeService employeeService;

        public HomeController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            IDashboardService dashboardService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.dashboardService = dashboardService;
        }

        public ActionResult Index()
        {
            IEnumerable<SummaryRequest> model = dashboardService.GetMySpace(sessionService.Employee.Id, DateTime.Now);
            ViewBag.IsResetPassword = employeeService.CheckResetPassword(sessionService.Employee.Id);
            ViewBag.IsVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);

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