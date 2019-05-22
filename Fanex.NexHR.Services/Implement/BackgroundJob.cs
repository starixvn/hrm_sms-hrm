namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.BackgroundJob;

    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBaseService baseService;

        public BackgroundJobService(IBaseService baseService)
        {
            this.baseService = baseService;
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
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddMonths(4).AddDays(-1);
            var criteria = new InsertScheduleByWeeklyDefaultShiftCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            baseService.ExecuteNonQuery(criteria);
        }
    }
}