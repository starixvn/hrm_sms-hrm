namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;

    [HRMPermission(Policy.Read, FunctionKey.WorkingCalendarReport)]
    public class WorkingCalendarSummaryReportController : BaseController
    {
        private readonly IWorkingCalendarSummaryReportService reportService;

        public WorkingCalendarSummaryReportController(
            ISessionService sessionService,
            IWorkingCalendarSummaryReportService reportService) : base(sessionService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Today;
            var model = new WorkingCalendarSummaryViewModel
            {
                FromDate = today,
                ToDate = today,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/WorkingCalendarSummary.cshtml", model);
        }

        [HttpGet]
        public ActionResult ExportExcel(WorkingCalendarSummaryViewModel model)
        {
            var param = new GetWorkingHoursSummaryParam
            {
                ActorId = sessionService.Employee.Id,
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
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}