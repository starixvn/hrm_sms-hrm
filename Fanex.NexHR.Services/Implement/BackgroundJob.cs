namespace Fanex.NexHR.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.BackgroundJob;
    using Fanex.NexHR.Models.BackgroundJob;
    using Hangfire;

    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BackgroundJobService));

        private readonly string messageEmailTemplate = "~/app_data/emails/RemindUseAL.cshtml";

        private readonly IBaseService baseService;
        private readonly IHolidayService holidayService;
        private readonly ISyncWorkingShiftService syncWorkingShiftService;
        private readonly IEmailService emailService;

        public BackgroundJobService(IBaseService baseService,
            IHolidayService holidayService,
            ISyncWorkingShiftService syncWorkingShiftService,
            IEmailService emailService)
        {
            this.baseService = baseService;
            this.holidayService = holidayService;
            this.syncWorkingShiftService = syncWorkingShiftService;
            this.emailService = emailService;
        }

        public void ImportTimeRecord()
        {
            var criteria = new ImportTimeRecordCriteria();
            baseService.ExecuteNonQuery(criteria);
        }

        public void PublishSchedule()
        {
            logger.Error("PublishSchedule: Start");

            var criteria = new PublishScheduleCriteria();
            baseService.ExecuteNonQuery(criteria);

            logger.Error("PublishSchedule: End");
        }

        public void InsertScheduleByDefaultShift()
        {
            logger.Error("InsertScheduleByDefaultShift: Start");

            var fromDate = DateTime.Now.Date.LastDayOfWeek().AddDays(1);
            var toDate = fromDate.AddDays(6);
            var criteria = new InsertScheduleByDefaultShiftCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            baseService.ExecuteNonQuery(criteria);

            logger.Error("InsertScheduleByDefaultShift: Start");
        }

        public void InsertScheduleByWeeklyDefaultShift()
        {
            logger.Error("InsertScheduleByWeeklyDefaultShift: Start");

            var fromDate = DateTime.Now.Date.LastDayOfWeek().AddDays(1);
            var toDate = fromDate.AddDays(6);
            var criteria = new InsertScheduleByWeeklyDefaultShiftCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            baseService.ExecuteNonQuery(criteria);

            logger.Error("InsertScheduleByWeeklyDefaultShift: Start");
        }

        public void InsertHolidayAutomatically()
        {
            logger.Error("InsertHolidayAutomatically: Start");

            var year = DateTime.Now.Year + 1;
            holidayService.AutoInsertHolidayOnYear(year);

            logger.Error("InsertHolidayAutomatically: End");
        }

        public void UpdateWorkingHourPerDay()
        {
            logger.Error("UpdateWorkingHourPerDay: Start");

            var criteria = new UpdateWorkingHourPerDayCriteria();
            baseService.ExecuteNonQuery(criteria);

            logger.Error("UpdateWorkingHourPerDay: End");
        }

        public void UpdateAnnualLeaveBalance()
        {
            logger.Error("UpdateAnnualLeaveBalance: Start");

            var criteria = new UpdateAnnualLeaveBalanceCriteria
            {
                Year = DateTime.Now.Year + 1
            };

            baseService.ExecuteNonQuery(criteria);

            logger.Error("UpdateAnnualLeaveBalance: End");
        }

        public void UpdateAnnualLeaveBalanceForNewEmployee()
        {
            logger.Error("UpdateAnnualLeaveBalanceForNewEmployee: Start");

            var criteria = new UpdateAnnualLeaveBalanceForNewEmployeeCriteria
            {
                Year = DateTime.Now.Year
            };
            baseService.ExecuteNonQuery(criteria);

            logger.Error("UpdateAnnualLeaveBalanceForNewEmployee: End");
        }

        public async Task SyncWorkingShiftAsync()
        {
            await syncWorkingShiftService.SyncWorkingShiftAsync();
        }

        #region Auto send mail remind user to use AL

        public void SendMailRemindUserToUseAnnualLeave()
        {
            logger.Error("SendMailRemindUserToUseAnnualLeave: Start");

            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = DateTime.Now.Year
            };
            var employees = baseService.GetData<AnnualLeaveBalance>(criteria);
            var bodyTemplate = Utilities.ReadTextFile(Path.GetFullPath(HostingEnvironment.MapPath(messageEmailTemplate)));

            foreach (var employee in employees)
            {
                float percentAnnualLeaveRemaining = employee.LeaveBalance / employee.AnnualLeaveCurrentYear * 100;
                bool isSentMail = false;
                var percentInMail = string.Empty;

                CheckSendMail(ref isSentMail, ref percentInMail, percentAnnualLeaveRemaining);

                if (!string.IsNullOrWhiteSpace(employee.Email) && isSentMail)
                {
                    var body = string.Format(
                        bodyTemplate,
                        employee.Nickname,
                        percentInMail,
                        DateTime.Now.Year + 1,
                        string.Format("{0}/{1}", employee.LeaveBalance, employee.AnnualLeaveCurrentYear));
                    BackgroundJob.Enqueue(() => emailService.SendEmail(
                        ConfigSettings.EmailSetting,
                        employee.Email,
                        string.Empty,
                        "Annual Leave Balance Reminder",
                        body));
                }
            }

            logger.Error("SendMailRemindUserToUseAnnualLeave: End");
        }

        private static void CheckSendMail(ref bool isSentMail, ref string percentInMail, float percentAnnualLeaveRemaining)
        {
            const byte March = 3;
            const byte June = 6;
            const byte September = 9;
            const byte December = 12;
            const byte TwentyFive = 25;
            const byte Fifty = 50;
            const byte SeventyFive = 75;
            switch (DateTime.Now.Month)
            {
                case March:
                    if (percentAnnualLeaveRemaining > SeventyFive)
                    {
                        isSentMail = true;
                        percentInMail = "over 75% ";
                    }
                    break;

                case June:
                    if (percentAnnualLeaveRemaining > Fifty)
                    {
                        isSentMail = true;
                        percentInMail = "over 50% ";
                    }
                    break;

                case September:
                    if (percentAnnualLeaveRemaining > TwentyFive)
                    {
                        isSentMail = true;
                        percentInMail = "over 25% ";
                    }
                    break;

                case December:
                    if (percentAnnualLeaveRemaining > 0)
                    {
                        isSentMail = true;
                    }
                    break;

                default:
                    // nothing
                    break;
            }
        }

        #endregion Auto send mail remind user to use AL
    }
}