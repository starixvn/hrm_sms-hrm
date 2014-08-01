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
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.ViewModels;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetWorkingHoursList(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(ICriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(
            int year,
            int? empId,
            string groupIds,
            byte? isLocal);

        IList<TimeRecordModel> GetTimeRecords(int? employeeId, string groupIds, DateTime fromDate, DateTime toDate, byte? isLocal);

        IList<WorkingCalendarSummary> CalendarSummaryList(ICriteria criteria);

        List<SupervisorReportGroupModel> GetSupervisorReport(ICriteria criteria);

        List<TeamSupervisorReportModel> GetAllTeamSupervisor(ICriteria criteria);
    }
}
