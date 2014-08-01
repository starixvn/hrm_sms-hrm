// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Report.Attendance;
    using Fanex.NexHR.Criteria.Report.ExtensionNumber;
    using Fanex.NexHR.Criteria.Report.Supervisor;
    using Fanex.NexHR.Criteria.Report.TimeRecord;
    using Fanex.NexHR.Criteria.Report.WorkingCalendar;
    using Fanex.NexHR.Criteria.Report.WorkingHour;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Service.
    /// </summary>
    public class ReportService : BaseService, IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService()
        {
            this._reportRepository = new ReportRepository();
        }

        public IList<TotalWorkingHours> GetWorkingHoursList(GetReportWorkingHourSummaryCriteria criteria) 
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetWorkingHoursList(criteria);
            }

            return totalWorkingHours;
        }

        /// <summary>
        /// Get Total Working Hour Monthly.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of TotalWorkingHourMonthly.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(GetTotalWorkingHourMonthlyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TotalWorkingHourMonthly> totalWorkingHours = null;
            if (criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetTotalWorkingHourMonthly(criteria);
            }

            return totalWorkingHours;
        }

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(GetTotalWorkingHourWeeklyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TotalWorkingHourMonthly> totalWorkingHours = null;
            if (criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetTotalWorkingHourWeekly(criteria);
            }

            return totalWorkingHours;
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitoringCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestsMonitoring> requestMonitoring = null;
            if (criteria.IsValid())
            {
                requestMonitoring = _reportRepository.GetRequestMonitoring(criteria);
            }

            return requestMonitoring;
        }

        /// <summary>
        /// Get Employee Attendance Report.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of EmployeeAttendanceModel.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EmployeeAttendanceModel> employeeAttendance = null;
            if (criteria.IsValid())
            {
                employeeAttendance = _reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return employeeAttendance;
        }

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(
            int year,
            int? empId,
            string groupIds,
            byte? isLocal)
        {
            var annualLeaveBalance = _reportRepository.GetAnnualLeaveBalanceReport(year, empId, groupIds, isLocal);

            return annualLeaveBalance;
        }

        #region Get leave summary by team

        /// <summary>
        /// This function use for sorting leave summaries. Any leave summary of employee will be grouped by team. (For easier displaying in view).
        /// </summary>
        /// <param name="leaveSummaryList">Leave summary List.</param>
        /// <param name="teams">Team List.</param>
        /// <returns>List Annual Leave.</returns>        
        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = AddManagementTeamFirstPosition(teams);

            foreach (Team team in teams)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList)
                {
                    if (leavesumary.Employee.Team.Name == team.Name)
                    {
                        /*Check if team is mamagement team, add leave summary for them*/
                        if ((team.Id == 11 && teams.Count > 1) || (teams.Count == 1 && teams[0].Id == 11))
                        {
                            leaveSummaryByTeamList[0].LeaveSummaryInTeam.Add(leavesumary);
                        }
                        else
                        {
                            entity.LeaveSummaryInTeam.Add(leavesumary);
                        }
                    }
                }

                // When select management team, return here
                if (teams.Count == 1 && teams[0].Id == 11)
                {
                    return leaveSummaryByTeamList;
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
        }
        #endregion

        /// <summary>
        /// Get all extension number.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator ExtensionNoModel.</returns>
        public IEnumerable<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<ExtensionNoModel> extensionNumbers = null;

            if (criteria.IsValid())
            {
                extensionNumbers = GetData<ExtensionNoModel>(criteria);
            }

            return extensionNumbers;
        }

        /// <summary>
        /// Get all extension group info.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator Extension No Model.</returns>
        public IEnumerable<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<ExtensionNoModel> groupsInfo = null;

            if (criteria.IsValid())
            {
                groupsInfo = GetData<ExtensionNoModel>(criteria);
            }

            return groupsInfo;
        }

        /// <summary>
        /// Insert new extension number member.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command result.</returns>
        public CommandResult AddNewExtensionNoMember(InsertExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get extension number of employee.
        /// </summary>
        /// <param name="criteria">THe criteria.</param>
        /// <returns>The extension number.</returns>
        public ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            ExtensionNoModel extensionNumber = new ExtensionNoModel();

            if (criteria.IsValid())
            {
                extensionNumber = GetDataEntity<ExtensionNoModel>(criteria);
            }

            return extensionNumber;
        }

        /// <summary>
        /// Update extension number of member.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult UpdateExtensionNoMember(UpdateExtensionNumberMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Delete extension number of member.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult DeleteExtensionNumber(DeleteExtensionNumberMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Insert new external group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult AddNewExternalGroup(InsertExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Update external group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult EditExternalGroup(UpdateExternalNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Delete external group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IList<TimeRecordModel> GetTimeRecords(int? employeeId, string groupIds, DateTime fromDate, DateTime toDate, byte? isLocal)
        {
            IList<TimeRecordModel> timeRecords = null;

            if (fromDate <= toDate)
            {
                timeRecords = _reportRepository.GetTimeRecords(employeeId, groupIds, fromDate, toDate, isLocal);
            }

            return timeRecords;
        }

        public IList<WorkingCalendarSummary> CalendarSummaryList(GetEmployeeWorkingCalendarByDateCriteria criteria)
        {
            IList<WorkingCalendarSummary> calendars = null;
            if (criteria.IsValid())
            {
                calendars = _reportRepository.CalendarSummaryList(criteria);
            }

            return calendars;
        }

        public List<SupervisorReportGroupModel> GetSupervisorReport(GetSupervisorReportCriteria criteria)
        {
            List<SupervisorReportGroupModel> groupingSupervisors = null;
            if (criteria.IsValid())
            {
                groupingSupervisors = _reportRepository.GetSupervisorReport(criteria);
            }

            return groupingSupervisors;
        }

        public List<TeamSupervisorReportModel> GetAllTeamSupervisor(GetAllTeamSupervisorCriteria criteria)
        {
            List<TeamSupervisorReportModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = _reportRepository.GetAllTeamSupervisor(criteria);
            }

            return supervisors;
        }
        
        public CommandResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Add management team in first position.
        /// </summary>
        /// <param name="teams">The list team.</param>
        /// <returns>The list Leave Summary by Team.</returns>
        private IList<LeaveSummaryByTeam> AddManagementTeamFirstPosition(
            IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList  = new List<LeaveSummaryByTeam>();

            if (teams.Count > 1 || (teams.Count == 1 && teams[0].Id == 11))
            {
                var teamService = new TeamService();
                var getGroupByIdCriteria = new GetGroupByIdCriteria
                {
                    GroupId = 11
                };
                LeaveSummaryByTeam management = new LeaveSummaryByTeam
                {
                    Team = teamService.GetById(getGroupByIdCriteria),
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                // Add management team in 1st position
                leaveSummaryByTeamList.Add(management);
            }

            return leaveSummaryByTeamList;
        }
    }
}
