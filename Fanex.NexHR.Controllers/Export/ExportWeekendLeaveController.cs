namespace Fanex.NexHR.Controllers.Export
{
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels.Report;
    using Criteria.Report;
    using Models.Report;
    using Services;
    using Services.ExportExcel.WeekendLeave;

    public class ExportWeekendLeaveController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExportWeekendLeaveService exportWeekendLeaveService;

        public ExportWeekendLeaveController()
            : this(null, null)
        {
        }

        public ExportWeekendLeaveController(
                            IReportService reportService,
                            IExportWeekendLeaveService exportWeekendLeaveService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportWeekendLeaveService = exportWeekendLeaveService ?? new ExportWeekendLeaveService();
        }

        public ActionResult ExportWeekendLeaveSummaryReport(WeekendLeaveSummaryViewModel model)
        {
            var criteria = new GetWeekendLeaveSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
            };

            IList<WeekendLeaveSummary> weekendLeaves = reportService.GetWeekendLeaveSummary(criteria);
            exportWeekendLeaveService.ExportWeekendLeaveSummay(criteria, weekendLeaves);

            return new EmptyResult();
        }
    }
}