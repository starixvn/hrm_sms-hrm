namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.Calendar;
    using System.Collections.Generic;

    public interface IWorkingCalendarService
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(GetWorkingCalendarsForShiftArrangeCriteria criteria);

        IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria);

        IList<EmployeeShiftModel> GetEmployeeShift(GetEmployeeShiftDateCriteria criteria);

        IList<WorkingScheduleViewModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria);

        IList<SupervisorModel> GetEmployeeByShiftManagement(GetEmployeeByShiftManagementCriteria criteria);
    }
}
