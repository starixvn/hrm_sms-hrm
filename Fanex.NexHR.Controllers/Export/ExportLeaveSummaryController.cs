namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    public class ExportLeaveSummaryController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly ICommonDataService commonDataService;
        private readonly IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService;

        public ExportLeaveSummaryController(
            ISessionService sessionService,
            IReportService reportService,
            IExportExcelService exportExcelService,
            ICommonDataService commonDataService,
            IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService) : base(sessionService)
        {
            this.reportService = reportService;
            this.exportExcelService = exportExcelService;
            this.commonDataService = commonDataService;
            this.exportExcelUnpaidLeaveSummaryReportService = exportExcelUnpaidLeaveSummaryReportService;
        }

        public ActionResult AnnualLeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = "LeaveSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            byte? isLocal = null;
            if (model.Category != null)
            {
                isLocal = (byte)model.Category;
            }

            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                TeamIds = model.TeamIds,
                IsLocal = isLocal,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> leaveSummary = reportService.GetAnnualLeaveBalanceReport(criteria);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportLeaveSummary(leaveSummary, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel model)
        {
            var criteria = new GetUnpaidLeaveCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                TeamIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = sessionService.Employee.Id,
                LeaveType = model.LeaveType
            };
            IList<LeaveSummary> leaveSummary = reportService.GetUnpaidLeave(criteria);
            exportExcelUnpaidLeaveSummaryReportService.Export(leaveSummary, model.Year, model.LeaveType, model.Period);

            return new EmptyResult();
        }
    }
}