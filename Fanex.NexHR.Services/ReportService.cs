// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.TimeRecord;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Report;

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

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourDaily(GetTotalWorkingHourDailyCriteria criteria)
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

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LeaveSummary> annualLeaveBalance = null;
            if (criteria.IsValid())
            {
                annualLeaveBalance = _reportRepository.GetAnnualLeaveBalanceReport(criteria);
            }

            return annualLeaveBalance;
        }

        public IList<LeaveSummary> GetUnpaidLeave(GetUnpaidLeaveCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LeaveSummary> unpaidLeaves = null;
            if (criteria.IsValid())
            {
                unpaidLeaves = _reportRepository.GetUnpaidLeave(criteria);
            }

            return unpaidLeaves;
        }

        public IList<LeaveSummaryByTeam> GetLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = AddManagementTeamFirstPosition(teams);

            foreach (Team team in teams)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList.Where(x => x.Employee.Team.Name == team.Name))
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

                // When select management team, return here
                if (teams.Count == 1 && teams[0].Id == 11)
                {
                    return leaveSummaryByTeamList;
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
        }

        public IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TimeRecordModel> timeRecords = null;
            if (criteria.IsValid())
            {
                timeRecords = _reportRepository.GetTimeRecords(criteria);
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

        public IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorReportModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = _reportRepository.GetSupervisorReport(criteria);
            }

            return supervisors;
        }

        public IList<TeamSupervisorReportModel> GetAllTeamSupervisor(GetAllTeamSupervisorCriteria criteria)
        {
            IList<TeamSupervisorReportModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = _reportRepository.GetAllTeamSupervisor(criteria);
            }

            return supervisors;
        }

        public IList<ManpowerModel> GetManpower(GetManpowerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<ManpowerModel> manpowers = null;

            if (criteria.IsValid())
            {
                manpowers = GetData<ManpowerModel>(criteria).ToList();
            }

            return manpowers;
        }

        public IList<ManpowerCategoryModel> GetManpowerCategory(GetManpowerCategoryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<ManpowerCategoryModel> manpowers = null;

            if (criteria.IsValid())
            {
                manpowers = GetData<ManpowerCategoryModel>(criteria).ToList();
            }

            return manpowers;
        }

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
