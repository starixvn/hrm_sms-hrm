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
    using Fanex.NexHR.Criteria.Report.ExtensionNumber;
    using Fanex.NexHR.Criteria.Report.Supervisor;
    using Fanex.NexHR.Criteria.Report.TimeRecord;
    using Fanex.NexHR.Criteria.Report.WorkingCalendar;
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

        public IList<TotalWorkingHours> GetWorkingHoursList(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            string groupIds,
            byte? isLocal = null) 
        {
            var totalWorkingHours = _reportRepository.GetWorkingHoursList(fromDate, toDate, empId, groupIds, isLocal);

            return totalWorkingHours;
        }

        /// <summary>
        /// Gets the total working hour monthly.
        /// </summary>
        /// <param name="month">The month will be filtered.</param>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="groupIds">The group ids.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>The list of TotalWorkingHourMonthly.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(
            DateTime month,
            int? empId,
            string groupIds,
            byte? isLocal = null)
        {
            try
            {
                var totalWorkingHourMonthly = _reportRepository.GetTotalWorkingHourMonthly(month, empId, groupIds, isLocal);
                return totalWorkingHourMonthly;
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
                return null;
            }
        }

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(
            int? empId,
            DateTime fromTime,
            DateTime toTime,
            string groupIds,
            byte? isLocal = null)
        {
            IList<TotalWorkingHourMonthly> totalWorkingHourWeekly = null;
            totalWorkingHourWeekly = _reportRepository.GetTotalWorkingHourWeekly(empId, fromTime, toTime, groupIds, isLocal);
            return totalWorkingHourWeekly;
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role,
            int? employeeId,
            int loginUserId,
            string groupIds,
            byte? isLocal = null)
        {
            var requestMonitoring = _reportRepository.GetRequestMonitoring(fromDate, toDate, maxRequest, role, employeeId, loginUserId, groupIds, isLocal);

            return requestMonitoring;
        }

        /// <summary>
        /// Function to get employeeAttendance Report.
        /// </summary>
        /// <param name="fromTime">From time to query DB.</param>
        /// <param name="toTime">To time to query DB..</param>
        /// <param name="type">Type of request.</param>
        /// <param name="empId">Employee Id.</param>
        /// <param name="groupIds">Group list.</param>
        /// <param name="excludeDeletedUsers">Is exclude user deleted.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>EmployeeAttendanceModel List object.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeDeletedUsers = 1,
            byte? isLocal = null)
        {
            var employeeAttendance = _reportRepository.GetEmployeeAttendanceReport(
                fromTime, 
                toTime,
                type,
                empId,
                groupIds,
                excludeDeletedUsers,
                isLocal);

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
