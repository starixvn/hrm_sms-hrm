namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarRepository
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(ICriteria criteria);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(ICriteria criteria);

        IList<WorkingScheduleViewModel> GetWorkingSchedule(ICriteria criteria, DateTime workingDate);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria);

        IList<EmployeeShiftModel> GetEmployeeShift(ICriteria criteria);
    }
}