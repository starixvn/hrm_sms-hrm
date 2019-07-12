namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Criteria.Report;
    using Fanex.NexHR.Core.Services;
    using Models;
    using NPOI.HSSF.UserModel;
    using Services;
    using ViewModels;

    public class ExportTotalWorkingHourController : BaseController
    {
        private readonly IReportService reportService;

        public ExportTotalWorkingHourController(
            ISessionService sessionService,
            IReportService reportService) : base(sessionService)
        {
            this.reportService = reportService;
        }

        public ActionResult TotalWorkingHours(ReportFilterViewModel workingHoursModel)
        {
            IExportService exportService;
            var model = workingHoursModel;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            const byte CurrentMonthOption = 5;
            if (model == null)
            {
                model = new ReportFilterViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = CurrentMonthOption
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