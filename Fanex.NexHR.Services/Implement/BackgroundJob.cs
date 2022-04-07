namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models;
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.BackgroundJob;

    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly string messageEmailTemplate = "~/app_data/emails/RemindUseAL.cshtml";

        private readonly IBaseService baseService;
        private readonly IHolidayService holidayService;
        private readonly ISyncWorkingShiftService syncWorkingShiftService;
        private readonly ITeamService teamService;
        private readonly IEmailService emailService;
        private readonly IReportService reportService;

        public BackgroundJobService(IBaseService baseService,
            IHolidayService holidayService,
            ISyncWorkingShiftService syncWorkingShiftService,
            ITeamService teamService,
            IEmailService emailService,
            IReportService reportService)
        {
            this.baseService = baseService;
            this.holidayService = holidayService;
            this.syncWorkingShiftService = syncWorkingShiftService;
            this.teamService = teamService;
            this.emailService = emailService;
            this.reportService = reportService;
        }

        public void ImportTimeRecord()
        {
            var criteria = new ImportTimeRecordCriteria();
            baseService.ExecuteNonQuery(criteria);
        }

        public void PublishSchedule()
        {
            var criteria = new PublishScheduleCriteria();
            baseService.ExecuteNonQuery(criteria);
        }

        public void InsertScheduleByDefaultShift()
        {
            var fromDate = DateTime.Now.Date.LastDayOfWeek().AddDays(1);
            var toDate = fromDate.AddDays(6);
            var criteria = new InsertScheduleByDefaultShiftCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            baseService.ExecuteNonQuery(criteria);
        }

        public void InsertScheduleByWeeklyDefaultShift()
        {
            var fromDate = DateTime.Now.Date.LastDayOfWeek().AddDays(1);
            var toDate = fromDate.AddDays(6);
            var criteria = new InsertScheduleByWeeklyDefaultShiftCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            baseService.ExecuteNonQuery(criteria);
        }

        public void InsertHolidayAutomatically()
        {
            var year = DateTime.Now.Year + 1;
            holidayService.AutoInsertHolidayOnYear(year);
        }

        public void UpdateWorkingHourPerDay()
        {
            var criteria = new UpdateWorkingHourPerDayCriteria();

            baseService.ExecuteNonQuery(criteria);
        }

        public void UpdateAnnualLeaveBalance()
        {
            var criteria = new UpdateAnnualLeaveBalanceCriteria
            {
                Year = DateTime.Now.Year + 1
            };

            baseService.ExecuteNonQuery(criteria);
        }

        public void UpdateAnnualLeaveBalanceForNewEmployee()
        {
            var criteria = new UpdateAnnualLeaveBalanceForNewEmployeeCriteria
            {
                Year = DateTime.Now.Year
            };

            baseService.ExecuteNonQuery(criteria);
        }

        public async Task SyncWorkingShiftAsync()
        {
            await syncWorkingShiftService.SyncWorkingShiftAsync();
        }

        #region Auto send mail remind user to use AL
        public void SendMailRemindUserToUseAnnualLeave()
        {

            IList<LeaveSummary> leaveSumaries = GetLeaveSumaries();
            var bodyTemplate = Utilities.ReadTextFile(Path.GetFullPath(HostingEnvironment.MapPath(messageEmailTemplate)));

            foreach (var leave in leaveSumaries)
            {
                float percentAnnualLeaveRemaining = leave.LeaveBalance / leave.AnnualLeaveCurrentYear * 100;
                bool isSentMail = false;
                string percentInMail = "";

                CheckSendMail(ref isSentMail, ref percentInMail, percentAnnualLeaveRemaining);

                if (leave.Email != null && isSentMail)
                {
                    var body = string.Format(bodyTemplate, leave.Nickname, percentInMail, DateTime.Now.Year + 1, leave.LeaveBalance + "/" + leave.AnnualLeaveCurrentYear);
                    emailService.SendEmail("mailservice@starixvn.com", leave.Email, "", "Annual Leave Balance Reminder", body);
                }
            }
        }

        private IList<LeaveSummary> GetLeaveSumaries()
        {
            IList<Team> teams = teamService.GetAll(false, null);
            string teamIds = string.Join(",", teams.Select(x => x.Id));

            var criteriaGetAnnualLeaveBalance = new GetAnnualLeaveBalanceCriteria
            {
                Year = DateTime.Now.Year,
                EmpId = null,
                TeamIds = teamIds,
                IsLocal = null,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> leaveSumaries = reportService.GetAnnualLeaveBalanceReport(criteriaGetAnnualLeaveBalance);
            return leaveSumaries;
        }

        private void CheckSendMail(ref bool isSentMail, ref string percentInMail, float percentAnnualLeaveRemaining)
        {
            switch (DateTime.Now.Month)
            {
                case 3:
                    if (percentAnnualLeaveRemaining > 75)
                    {
                        isSentMail = true;
                        percentInMail = "over 75% ";
                    }
                    break;
                case 6:
                    if (percentAnnualLeaveRemaining > 50)
                    {
                        isSentMail = true;
                        percentInMail = "over 50% ";
                    }
                    break;
                case 9:
                    if (percentAnnualLeaveRemaining > 25)
                    {
                        isSentMail = true;
                        percentInMail = "over 25% ";
                    }
                    break;
                case 12:
                    if (percentAnnualLeaveRemaining > 0)
                    {
                        isSentMail = true;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}