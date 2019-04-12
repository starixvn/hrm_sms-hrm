namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarService
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria);

        IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria);

        IList<EmployeeShiftModel> GetEmployeeShift(GetEmployeeShiftDateCriteria criteria);

        IList<WorkingScheduleViewModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria);

        IList<SupervisorModel> GetEmployeeByShiftManagement(GetEmployeeByShiftManagementCriteria criteria);
    }
}