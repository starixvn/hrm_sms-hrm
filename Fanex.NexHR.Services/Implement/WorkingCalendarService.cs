namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;

    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly IWorkingCalendarRepository workingCalendarRepository;

        public WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository)
        {
            this.workingCalendarRepository = workingCalendarRepository;
        }

        public IEnumerable<DailySchedule> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria)
        {
            return workingCalendarRepository.GetWorkingSchedule(criteria);
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria)
        {
            return workingCalendarRepository.GetWorkingCalendars(criteria);
        }
    }
}