namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    public class ExportCalendarController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IWorkingCalendarService workingCalendarService;

        public ExportCalendarController(
            ISessionService sessionService,
            IExportExcelService exportExcelService,
            IWorkingCalendarService workingCalendarService) : base(sessionService)
        {
            this.exportExcelService = exportExcelService;
            this.workingCalendarService = workingCalendarService;
        }

        public ActionResult EmployeeWorkingCalendar(WorkingCalendarFilterViewModel model)
        {
            string fileName = "EmployeeWorkingCalendar_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetWorkingCalendarWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                TeamIds = model.TeamIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };

            IEnumerable<WorkingCalendar> workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWorkingCalendar(workingCalendars, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}