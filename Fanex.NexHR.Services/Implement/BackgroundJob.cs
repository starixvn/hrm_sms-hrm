namespace Fanex.NexHR.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.BackgroundJob;
    using Fanex.NexHR.Services.Implement;

    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBaseService baseService;
        private readonly IHolidayService holidayService;
        private readonly ISyncWorkingShiftService syncWorkingShiftService;

        public BackgroundJobService(IBaseService baseService, IHolidayService holidayService, ISyncWorkingShiftService syncWorkingShiftService)
        {
            this.baseService = baseService;
            this.holidayService = holidayService;
            this.syncWorkingShiftService = syncWorkingShiftService;
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
    }
}