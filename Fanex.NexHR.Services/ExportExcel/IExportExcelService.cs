namespace Fanex.NexHR.Services.ExportExcel
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Calendar;
    using Fanex.NexHR.ViewModels.Report;
    using NPOI.HSSF.UserModel;

    public interface IExportExcelService
    {
        HSSFWorkbook ExportAttendance(IList<EmployeeAttendanceModel> attendanceList, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year);

        HSSFWorkbook ExportUnpaidLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year);

        HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList);

        void ExportOvertimeReport(IList<OvertimeDaily> requestList, DateTime fromDate, DateTime toDate, string templatePath);

        HSSFWorkbook ExportYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> requestList, int year, int? calendar);

        void ExportEmployeeBasicInfo(IList<Employee> employees, string templatePath);

        void ExportEmployeeDetailsInfo(IList<Employee> employees, string templatePath);

        HSSFWorkbook ExportEmployeeWorkingCalendar(IList<WorkingCalendarsViewModel> workingCalendars, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportEmployeeOvertimeRecord(IList<OvertimeExcelReport> overtimes, int? calendar, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportWorkingCalendarSummary(List<WorkingCalendarSummary> calendars, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportHRRequestStatistic(List<RequestsMonitoring> requests, DateTime fromDate, DateTime toDate);

        void ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors, string templatePath);

        void ExportSectionSupervisor(IList<SectionSupervisorReportModel> supervisors, string templatePath);

        void ExportManpower(IList<ManpowerModel> manpowers, int year, string templatePath);

        void ExportManpowerCategory(IList<ManpowerCategoryModel> manpowers, int year, string templatePath);
    }
}