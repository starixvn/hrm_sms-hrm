namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarService
    {
        IEnumerable<DailySchedule> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria);
    }
}