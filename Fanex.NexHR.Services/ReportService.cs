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
    using Fanex.NexHR.Criteria.Report.Supervisor;
    using Fanex.NexHR.Criteria.Report.WorkingCalendar;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Service.
    /// </summary>
    public class ReportService : IReportService
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
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();
            if (teams.Count > 1 || (teams.Count == 1 && teams[0].Id == 11))
            {
                var teamService = new TeamService();
                LeaveSummaryByTeam management = new LeaveSummaryByTeam
                {
                    Team = teamService.Get(11),
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                // Add management team in 1st position
                leaveSummaryByTeamList.Add(management);
            }

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

        public IList<ExtensionNoModel> GetAllExtensionNo()
        {
            return _reportRepository.GetAllExtensionNo();
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(bool isInternal, int? groupId)
        {
            return _reportRepository.GetAllExtensionGroupInfo(isInternal, groupId);
        }

        public bool AddNewExtensionNoMember(ExtensionNoModel model)
        {
            int errorCode;
            string message;
            return _reportRepository.AddNewExtensionNoMember(model, out errorCode, out message);
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(bool isInternal, int empExtID)
        {
            return _reportRepository.GetEmployeeExtensionNoById(isInternal, empExtID);
        }

        public bool UpdateExtensionNoMember(ExtensionNoModel model)
        {
            int errorCode;
            string message;
            return _reportRepository.UpdateExtensionNoMember(model, out errorCode, out message);
        }

        public bool DeleteExtensionNumber(int extId)
        {
            int errorCode;
            string message;
            return _reportRepository.DeleteExtensionNumber(extId, out errorCode, out message);
        }

        public bool AddNewExternalGroup(string groupName)
        {
            int errorCode;
            string message;
            return _reportRepository.AddNewExternalGroup(groupName, out errorCode, out message);
        }

        public bool EditExternalGroup(int groupId, string groupName)
        {
            int errorCode;
            string message;
            return _reportRepository.EditExternalGroup(groupId, groupName, out errorCode, out message);
        }

        public bool DeleteExternalGroup(int groupId)
        {
            int errorCode;
            string message;
            return _reportRepository.DeleteExternalGroup(groupId, out errorCode, out message);
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
    }
}
