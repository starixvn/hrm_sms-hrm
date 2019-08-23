namespace Fanex.NexHR.Controllers
{
    using System;
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
    public class WorkingCalendarSummaryReportController : BaseController
    {
        private readonly IWorkingCalendarSummaryReportService reportService;
        private readonly ITeamService teamService;

        public WorkingCalendarSummaryReportController(
            ISessionService sessionService,
            IWorkingCalendarSummaryReportService reportService,
            ITeamService teamService) : base(sessionService)
        {
            this.reportService = reportService;
            this.teamService = teamService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Today;
            IEnumerable<Team> sections = teamService.GetAll();
            var model = new WorkingCalendarSummaryViewModel
            {
                FromDate = today,
                ToDate = today,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled,
                Sections = sections.Select(x => new EntityBase<int> { Id = x.Id, Name = x.Name, Code = x.Code })
            };

            return View("~/Views/Report/WorkingCalendarSummary.cshtml", model);
        }

        [HttpGet]
        public ActionResult ExportExcel(WorkingCalendarSummaryViewModel model)
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
            IEnumerable<WorkingCalendarSummary> calendars = reportService.GetWorkingCalendarSummary(param);
            byte[] bytes = reportService.Export(param, calendars);

            var fileName = $"Working Calendar Summary {model.FromDate.ToString("dd MMM yyyy")} - {model.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}