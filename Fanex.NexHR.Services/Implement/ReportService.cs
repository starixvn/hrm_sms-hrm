namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IBaseService baseService;

        public ReportService(
            IReportRepository reportRepository,
            IBaseService baseService)
        {
            this.reportRepository = reportRepository;
            this.baseService = baseService;
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
            var leaveSummaryByTeams = new List<LeaveSummaryByTeam>();

            foreach (Team team in teams)
            {
                var entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList.Where(x => x.Employee.Team.Name == team.Name))
                {
                    entity.LeaveSummaryInTeam.Add(leavesumary);
                }

                leaveSummaryByTeams.Add(entity);
            }

            return leaveSummaryByTeams;
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
                calendars = baseService.GetData<WorkingCalendarSummary>(criteria).ToList();
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
    }
}