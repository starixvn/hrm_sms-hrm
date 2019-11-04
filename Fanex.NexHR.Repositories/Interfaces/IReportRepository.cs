namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;

    public partial interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria);

        IList<LeaveSummary> GetUnpaidLeave(ICriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(ICriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria);

        IList<SectionSupervisorReportModel> GetSectionSupervisor(ICriteria criteria);
    }
}