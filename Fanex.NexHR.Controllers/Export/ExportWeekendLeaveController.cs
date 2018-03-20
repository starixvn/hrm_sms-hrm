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
        private readonly IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService;
        private readonly IExportWeekendLeaveDetailService exportWeekendLeaveDetailService;

        public ExportWeekendLeaveController()
            : this(null, null, null)
        {
        }

        public ExportWeekendLeaveController(
                            IReportService reportService,
                            IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService,
                            IExportWeekendLeaveDetailService exportWeekendLeaveDetailService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportWeekendLeaveSummaryService = exportWeekendLeaveSummaryService ?? new ExportWeekendLeaveSummaryService();
            this.exportWeekendLeaveDetailService = exportWeekendLeaveDetailService ?? new ExportWeekendLeaveDetailService();
        }

        public ActionResult ExportWeekendLeaveSummaryReport(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                MinTotalLeave = model.MinTotalLeave,
                SupervisorId = CurrentUser.Id
            };

            IList<WeekendLeaveSummary> weekendLeaves = reportService.GetWeekendLeaveSummary(criteria);
            exportWeekendLeaveSummaryService.Export(criteria, weekendLeaves);

            return new EmptyResult();
        }

        public ActionResult ExportWeekendLeaveDetailReport(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId
            };

            IList<WeekendLeaveDetail> weekendLeaveDetails = reportService.GetWeekendLeaveDetail(criteria);
            exportWeekendLeaveDetailService.Export(criteria, weekendLeaveDetails);

            return new EmptyResult();
        }
    }
}