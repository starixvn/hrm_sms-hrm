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
    using Fanex.NexHR.Models.Report.Overtime;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class WorkingHoursSummaryReportController : BaseController
    {
        private readonly IWorkingHoursSummaryReportService reportService;
        private readonly IOvertimeReportService overtimeReportService;
        private readonly ITeamService teamService;

        public WorkingHoursSummaryReportController(
            ISessionService sessionService,
            IWorkingHoursSummaryReportService reportService,
            IOvertimeReportService overtimeReportService,
            ITeamService teamService) : base(sessionService)
        {
            this.reportService = reportService;
            this.overtimeReportService = overtimeReportService;
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
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
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
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<WorkingHoursSummary> workingHours = reportService.GetWorkingHoursSummary(param);
            IEnumerable<DailyOvertimeDetails> overtimes = GetOvertimeDetails(model);
            byte[] bytes = reportService.Export(param, workingHours, overtimes);

            var fileName = $"Attendance Statistic for Payroll.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }

        private IEnumerable<DailyOvertimeDetails> GetOvertimeDetails(WorkingHoursSummaryViewModel model)
        {
            var param = new GetDailyOvertimeDetailsParam
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            return overtimeReportService.GetDailyOvertimeDetails(param);
        }
    }
}