namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarRepository
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(ICriteria criteria);

        IEnumerable<DailySchedule> GetWorkingSchedule(ICriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria);

        IList<EmployeeShiftModel> GetEmployeeShift(ICriteria criteria);
    }
}