namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.Report;
    using System.Collections.Generic;

    public interface IReportService
    {
        IList<TotalWorkingHours> GetWorkingHoursList(GetReportWorkingHourSummaryCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(GetTotalWorkingHourMonthlyCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourDaily(GetTotalWorkingHourDailyCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(GetTotalWorkingHourWeeklyCriteria criteria);
        
        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitoringCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria);

        IList<LeaveSummary> GetUnpaidLeave(GetUnpaidLeaveCriteria criteria);

        IList<LeaveSummaryByTeam> GetLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams);

        IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria);

        IList<WorkingCalendarSummary> CalendarSummaryList(GetEmployeeWorkingCalendarByDateCriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria);

        IList<SectionSupervisorReportModel> GetSectionSupervisor(GetSectionSupervisorCriteria criteria);

        IList<ManpowerModel> GetManpower(GetManpowerCriteria criteria);

        IList<ManpowerCategoryModel> GetManpowerCategory(GetManpowerCategoryCriteria criteria);
    }
}
