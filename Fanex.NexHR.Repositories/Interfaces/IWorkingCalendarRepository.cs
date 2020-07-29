namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IWorkingCalendarRepository
    {
        IEnumerable<DailySchedule> GetWorkingSchedule(ICriteria criteria);

        IEnumerable<WorkingCalendarRangeDates> GetWorkingCalendars(ICriteria criteria);
    }
}