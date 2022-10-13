namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Fanex.NexHR.Models.Dashboard;
    using Fanex.NexHR.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class HomeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDashboardService dashboardService;
        private readonly IEmployeeService employeeService;
        private readonly IUserService userService;

        public HomeController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            IDashboardService dashboardService,
            IUserService userService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.dashboardService = dashboardService;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            var user = userService.GetById(userControlSession.UserId);
            IEnumerable<SummaryRequest> model = dashboardService.GetMySpace(EmployeeSession.EmpId, DateTime.Now);
            var employeesLogin = dashboardService.GetCurrentEmployeeLogin().ToList();
            var excludeCurrentEmployee = employeesLogin.Where(x => x.EmpId != sessionService.Employee.Id);
            ViewBag.IsResetPassword = user.IsResetPassword;
            ViewBag.IsVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);
            ViewBag.CurrentEmployeeLogin = excludeCurrentEmployee;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult MemberRequest()
        {
            IEnumerable<MemberRequest> model = dashboardService.GetMemberRequest(EmployeeSession.EmpId, DateTime.Now);

            return PartialView("_MemberRequest", model);
        }
    }
}