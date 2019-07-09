namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.ExportExcel;
    using Fanex.NexHR.ViewModels.Report.Overtime;
    using NPOI.HSSF.UserModel;

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class OvertimeReportController : BaseController
    {
        private readonly IOvertimeRequestService overtimeService;
        private readonly IExportExcelService exportExcelService;

        public OvertimeReportController(
            ISessionService sessionService,
            IOvertimeRequestService overtimeService,
            IExportExcelService exportExcelService) : base(sessionService)
        {
            this.overtimeService = overtimeService;
            this.exportExcelService = exportExcelService;
        }

        public ActionResult MonthlyOvertime()
        {
            var today = DateTime.Today;
            var model = new MonthlyFilterViewModel
            {
                Month = today.Month,
                Year = today.Year,
                Calendar = (byte)CalendarType.Fiscal
            };

            return View("~/Views/Report/MonthlyOvertime.cshtml", model);
        }

        public ActionResult ExportMonthlyOvertime(MonthlyFilterViewModel model)
        {
            DateTime requestDate = new DateTime(model.Year, model.Month, 1);

            string fileName = "EmployeeOvertimeRecord_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetOvertimeRecordCriteria
            {
                Month = requestDate,
                EmployeeId = model.EmployeeId,
                Calendar = model.Calendar
            };
            IList<OvertimeExcelReport> overtimes = overtimeService.OvertimeRecord(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeOvertimeRecord(overtimes, model.Calendar, model.FromDate, model.ToDate);

            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }
    }
}