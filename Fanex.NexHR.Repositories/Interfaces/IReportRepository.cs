// --------------------------------------------------------------------------
// <copyright file="IReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.Report;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetWorkingHoursList(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria);

        IList<LeaveSummary> GetUnpaidLeave(ICriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(ICriteria criteria);

        IList<WorkingCalendarSummary> CalendarSummaryList(ICriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria);

        IList<TeamSupervisorReportModel> GetAllTeamSupervisor(ICriteria criteria);
    }
}
