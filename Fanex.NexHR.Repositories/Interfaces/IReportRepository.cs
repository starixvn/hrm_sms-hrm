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
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeDeletedUsers = 1,
            byte? isLocal = null);

        IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role,
            int? employeeId,
            int loginUserId,
            string groupIds,
            byte? isLocal = null);

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
