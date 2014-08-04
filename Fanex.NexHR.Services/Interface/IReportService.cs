// --------------------------------------------------------------------------
// <copyright file="IReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Report.Attendance;
    using Fanex.NexHR.Criteria.Report.ExtensionNumber;
    using Fanex.NexHR.Criteria.Report.Supervisor;
    using Fanex.NexHR.Criteria.Report.TimeRecord;
    using Fanex.NexHR.Criteria.Report.WorkingCalendar;
    using Fanex.NexHR.Criteria.Report.WorkingHour;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.ViewModels;

    public interface IReportService
    {
        IList<TotalWorkingHours> GetWorkingHoursList(GetReportWorkingHourSummaryCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(GetTotalWorkingHourMonthlyCriteria criteria);

        IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(GetTotalWorkingHourWeeklyCriteria criteria);
        
        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitoringCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams);

        IEnumerable<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria);

        IEnumerable<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria);

        CommandResult AddNewExtensionNoMember(InsertExtensionNumberCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria);

        CommandResult UpdateExtensionNoMember(UpdateExtensionNumberMemberCriteria criteria);

        CommandResult DeleteExtensionNumber(DeleteExtensionNumberMemberCriteria criteria);

        CommandResult AddNewExternalGroup(InsertExternalGroupCriteria criteria);

        CommandResult EditExternalGroup(UpdateExternalNumberCriteria criteria);

        CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(int? employeeId, string groupIds, DateTime fromDate, DateTime toDate, byte? isLocal);

        IList<WorkingCalendarSummary> CalendarSummaryList(GetEmployeeWorkingCalendarByDateCriteria criteria);

        List<SupervisorReportGroupModel> GetSupervisorReport(GetSupervisorReportCriteria criteria);

        List<TeamSupervisorReportModel> GetAllTeamSupervisor(GetAllTeamSupervisorCriteria criteria);
        
        CommandResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria);
    }
}
