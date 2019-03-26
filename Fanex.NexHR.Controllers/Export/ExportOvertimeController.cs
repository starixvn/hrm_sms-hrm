namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Criteria;
    using Models;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using ViewModels;

    public class ExportOvertimeController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IExportExcelOvertimeReportService exportExcelOvertimeReportService;
        private readonly IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService;

        public ExportOvertimeController(
            IExportExcelService exportExcelService,
            IOvertimeRequestService overtimeService,
            IExportExcelOvertimeReportService exportExcelOvertimeReportService,
            IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService)
        {
            this.exportExcelService = exportExcelService;
            this.overtimeService = overtimeService;
            this.exportExcelOvertimeReportService = exportExcelOvertimeReportService;
            this.exportExcelYearlyOvertimeReportService = exportExcelYearlyOvertimeReportService;
        }

        public ActionResult OvertimeReport(OvertimeDetailReportViewModel model)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = model.EmployeeId,
                JobTitleId = model.JobTitleIds,
                GroupIds = model.SectionIds
            };
            IList<OvertimeDaily> overtimeDetails = overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            exportExcelOvertimeReportService.Export(overtimeDetails, model.FromDate, model.ToDate);

            return new EmptyResult();
        }

        public ActionResult YearlyOvertimeReport(YearlyOvertimeReportViewModel model)
        {
            IList<OvertimeAnnualReportViewModel> entity = null;
            if (model.Calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorId = this.CurrentUser.Id,
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            exportExcelYearlyOvertimeReportService.Export(entity, model.Year, model.Calendar);

            return new EmptyResult();
        }

        public ActionResult EmployeeOvertimeReport(int? year, int? month, int? calendar, int? empId)
        {
            DateTime requestDate = new DateTime(year.Value, month.Value, 1);
            DateTime fromDate;
            DateTime toDate;

            if (calendar == 0)
            {
                var dayInMonth = DateTime.DaysInMonth(year.Value, month.Value);
                fromDate = requestDate;
                toDate = new DateTime(year.Value, month.Value, dayInMonth);
            }
            else
            {
                DateTime prevMonthDate = requestDate.Date.AddMonths(-1);
                fromDate = new DateTime(prevMonthDate.Year, prevMonthDate.Month, 10);
                toDate = new DateTime(requestDate.Year, requestDate.Month, 9);
            }

            string fileName = "EmployeeOvertimeRecord_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetOvertimeRecordCriteria
            {
                Month = requestDate,
                EmployeeId = empId,
                Calendar = calendar.Value
            };
            IList<OvertimeExcelReport> overtimes = overtimeService.OvertimeRecord(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeOvertimeRecord(overtimes, calendar, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}