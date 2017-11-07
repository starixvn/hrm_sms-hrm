namespace Fanex.NexHR.Controllers
{
    using Criteria.Report;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels;

    public class ExportAttendanceController: BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelAttendanceReportService exportExcelAttendanceReportService;

        public ExportAttendanceController(): this(null, null)
        {
        }

        public ExportAttendanceController(IReportService reportService, IExportExcelAttendanceReportService exportExcelAttendanceReportService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportExcelAttendanceReportService = exportExcelAttendanceReportService ?? new ExportExcelAttendanceReportService();
        }

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmployeeId,
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal
            };
            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);
            exportExcelAttendanceReportService.Export(employeeAttendances, model.FromDate, model.ToDate);

            return new EmptyResult();
        }
    }
}