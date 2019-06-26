namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;
    using Models.Report;

    public interface IReportService
    {
        IList<TotalWorkingHours> GetWorkingHoursList(GetReportWorkingHourSummaryCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourDaily(GetTotalWorkingHourDailyCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(GetTotalWorkingHourWeeklyCriteria criteria);

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

        IList<WeekendLeaveSummary> GetWeekendLeaveSummary(GetWeekendLeaveSummaryCriteria criteria);

        IList<WeekendLeaveDetail> GetWeekendLeaveDetail(GetWeekendLeaveDetailCriteria criteria);
    }
}