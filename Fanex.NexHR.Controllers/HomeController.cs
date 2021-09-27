namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Dashboard;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Implement;

    public class HomeController : BaseController
    {
        private readonly IDashboardService dashboardService;
        private readonly IEmployeeService employeeService;
        private readonly ISyncWorkingShiftService syncWorkingShiftService;

        public HomeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IDashboardService dashboardService,
            ISyncWorkingShiftService syncWorkingShiftService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.dashboardService = dashboardService;
            this.syncWorkingShiftService = syncWorkingShiftService;
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
