namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Calendar;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using ViewModels;

    public class ExportCalendarController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IWorkingCalendarService workingCalendarService;

        public ExportCalendarController(
            IExportExcelService exportExcelService,
            IWorkingCalendarService workingCalendarService)
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
                GroupIds = model.SectionIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };

            IList<WorkingCalendarsViewModel> workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWorkingCalendar(workingCalendars, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}