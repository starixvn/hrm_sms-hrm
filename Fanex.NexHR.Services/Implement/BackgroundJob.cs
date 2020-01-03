namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.BackgroundJob;

    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBaseService baseService;
        private readonly IHolidayService holidayService;

        public BackgroundJobService(IBaseService baseService, IHolidayService holidayService)
        {
            this.baseService = baseService;
            this.holidayService = holidayService;
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
            var nowYear = DateTime.Now.Year;
            holidayService.AutoInsertHolidayOnYear(nowYear);
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
    }
}