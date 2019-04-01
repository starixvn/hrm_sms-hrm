namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<TotalWorkingHours> GetWorkingHoursList(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria);

        IList<LeaveSummary> GetUnpaidLeave(ICriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(ICriteria criteria);

        IList<WorkingCalendarSummary> CalendarSummaryList(ICriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria);

        IList<SectionSupervisorReportModel> GetSectionSupervisor(ICriteria criteria);
    }
}