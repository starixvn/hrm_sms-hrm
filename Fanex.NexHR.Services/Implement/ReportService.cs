namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;
    using Models.Report;

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IBaseService baseService;
        private readonly ITeamService teamService;

        public ReportService(
            IReportRepository reportRepository,
            IBaseService baseService,
            ITeamService teamService)
        {
            this.reportRepository = reportRepository;
            this.baseService = baseService;
            this.teamService = teamService;
        }

        public IList<TotalWorkingHours> GetWorkingHoursList(GetReportWorkingHourSummaryCriteria criteria)
        {
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetWorkingHoursList(criteria);
            }

            return totalWorkingHours;
        }

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(GetTotalWorkingHourMonthlyCriteria criteria)
        {
            IList<TotalWorkingHourMonthly> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetTotalWorkingHourMonthly(criteria);
            }

            return totalWorkingHours;
        }

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourDaily(GetTotalWorkingHourDailyCriteria criteria)
        {
            IList<TotalWorkingHourMonthly> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetTotalWorkingHourMonthly(criteria);
            }

            return totalWorkingHours;
        }

        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(GetTotalWorkingHourWeeklyCriteria criteria)
        {
            IList<TotalWorkingHourMonthly> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetTotalWorkingHourWeekly(criteria);
            }

            return totalWorkingHours;
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitoringCriteria criteria)
        {
            IList<RequestsMonitoring> requestMonitoring = null;
            if (criteria != null && criteria.IsValid())
            {
                requestMonitoring = reportRepository.GetRequestMonitoring(criteria);
            }

            return requestMonitoring;
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria)
        {
            IList<EmployeeAttendanceModel> employeeAttendance = null;
            if (criteria != null && criteria.IsValid())
            {
                employeeAttendance = reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return employeeAttendance;
        }

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria)
        {
            IList<LeaveSummary> annualLeaveBalance = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeaveBalance = reportRepository.GetAnnualLeaveBalanceReport(criteria);
            }

            return annualLeaveBalance;
        }

        public IList<LeaveSummary> GetUnpaidLeave(GetUnpaidLeaveCriteria criteria)
        {
            IList<LeaveSummary> unpaidLeaves = null;
            if (criteria != null && criteria.IsValid())
            {
                unpaidLeaves = reportRepository.GetUnpaidLeave(criteria);
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
            IList<TimeRecordModel> timeRecords = null;
            if (criteria != null && criteria.IsValid())
            {
                timeRecords = reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords;
        }

        public IList<WorkingCalendarSummary> CalendarSummaryList(GetEmployeeWorkingCalendarByDateCriteria criteria)
        {
            IList<WorkingCalendarSummary> calendars = null;
            if (criteria != null && criteria.IsValid())
            {
                calendars = reportRepository.CalendarSummaryList(criteria);
            }

            return calendars;
        }

        public IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria)
        {
            IList<SupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = reportRepository.GetSupervisorReport(criteria);
            }

            return supervisors;
        }

        public IList<SectionSupervisorReportModel> GetSectionSupervisor(GetSectionSupervisorCriteria criteria)
        {
            IList<SectionSupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = reportRepository.GetSectionSupervisor(criteria);
            }

            return supervisors;
        }

        public IList<ManpowerModel> GetManpower(GetManpowerCriteria criteria)
        {
            IList<ManpowerModel> manpowers = null;
            if (criteria != null && criteria.IsValid())
            {
                manpowers = baseService.GetData<ManpowerModel>(criteria).ToList();
            }

            return manpowers;
        }

        public IList<ManpowerCategoryModel> GetManpowerCategory(GetManpowerCategoryCriteria criteria)
        {
            IList<ManpowerCategoryModel> manpowers = null;
            if (criteria != null && criteria.IsValid())
            {
                manpowers = baseService.GetData<ManpowerCategoryModel>(criteria).ToList();
            }

            return manpowers;
        }

        public IList<WeekendLeaveSummary> GetWeekendLeaveSummary(GetWeekendLeaveSummaryCriteria criteria)
        {
            return baseService.GetData<WeekendLeaveSummary>(criteria).ToList();
        }

        public IList<WeekendLeaveDetail> GetWeekendLeaveDetail(GetWeekendLeaveDetailCriteria criteria)
        {
            return baseService.GetData<WeekendLeaveDetail>(criteria).ToList();
        }

        private IList<LeaveSummaryByTeam> AddManagementTeamFirstPosition(
            IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();

            if (teams.Count > 1 || (teams.Count == 1 && teams[0].Id == 11))
            {
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