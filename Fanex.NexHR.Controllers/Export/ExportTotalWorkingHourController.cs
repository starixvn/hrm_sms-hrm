namespace Fanex.NexHR.Controllers
{
    using Criteria;
    using Criteria.Report;
    using Models;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using ViewModels;

    public class ExportTotalWorkingHourController : BaseController
    {
        private readonly IReportService reportService;

        public ExportTotalWorkingHourController()
            : this(null)
        {
        }

        public ExportTotalWorkingHourController(IReportService reportService)
        {
            this.reportService = reportService ?? new ReportService();
        }

        public ActionResult TotalWorkingHours(ReportFilterViewModel workingHoursModel)
        {
            IExportService exportService;
            var model = workingHoursModel;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            if (model == null)
            {
                model = new ReportFilterViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                };
            }

            string fileName = "TotalWorkingHours_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var getTotalWorkingHourCriteria = new GetReportWorkingHourSummaryCriteria
            {
                Fromdate = model.FromDate,
                Todate = model.ToDate,
                EmpId = (model.EmployeeId != -1 && model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = model.IsLocal
            };

            IList<TotalWorkingHours> totalWorkingHours = reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            exportService = new TotalWorkingHourExportService(totalWorkingHours, model.FromDate, model.ToDate);
            HSSFWorkbook hssfworkbook = exportService.Export();

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ExportTotalWorkingHourWeekly(TotalWorkingHourWeeklyViewModel model)
        {
            IExportService exportService;
            string fileName = "TotalWorkingHourWeekly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                FromDate = DateTime.Parse(model.FromDate),
                ToDate = DateTime.Parse(model.ToDate),
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal
            };
            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourWeekly(criteria);
            exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ExportTotalWorkingHourMonthly(TotalWorkingHourMonthlyViewModel model)
        {
            IExportService exportService;
            string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourMonthlyCriteria
            {
                EmpId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ReportMonth = new DateTime(model.Year, model.Month, 1)
            };

            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourMonthly(criteria);
            exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ExportTotalWorkingHourDaily(TotalWorkingHourDailyViewModel model)
        {
            IExportService exportService;
            string fileName = "TotalWorkingHourDaily_" + model.WorkDate.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourDailyCriteria
            {
                EmpId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                WorkDate = model.WorkDate
            };

            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourDaily(criteria);
            exportService = new TotalWorkingHourDailyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();

            return ExcelFile(hssfworkbook, fileName);
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }
    }
}