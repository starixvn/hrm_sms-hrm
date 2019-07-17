namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class WorkingHoursSummaryReportController : BaseController
    {
        private readonly IWorkingHoursSummaryReportService reportService;
        private readonly ITeamService teamService;

        public WorkingHoursSummaryReportController(
            ISessionService sessionService,
            IWorkingHoursSummaryReportService reportSevice,
            ITeamService teamService) : base(sessionService)
        {
            this.reportService = reportSevice;
            this.teamService = teamService;
        }

        [HttpGet]
        public ActionResult Index(WorkingHoursSummaryViewModel model)
        {
            IEnumerable<Team> sections = teamService.GetAll();
            model.Sections = sections.Select(x => new EntityBase<int> { Id = x.Id, Name = x.Name, Code = x.Code });
            var param = new GetWorkingHoursSummaryParam
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                IsLocal = model.IsLocal
            };
            model.WorkingHours = reportService.GetWorkingHoursSummary(param);

            ViewBag.IsHR = sessionService.HasPermissionOnFunction(FunctionKey.ProcessRequest, Policy.Host);

            return View("~/Views/Report/WorkingHoursSummary.cshtml", model);
        }

        [HttpGet]
        public ActionResult ExportExcel(WorkingHoursSummaryViewModel model)
        {
            var param = new GetWorkingHoursSummaryParam
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                IsLocal = model.IsLocal
            };
            IEnumerable<WorkingHoursSummary> workingHours = reportService.GetWorkingHoursSummary(param);
            byte[] bytes = reportService.Export(param, workingHours);

            var fileName = $"Attendance Statistic for Payroll.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}