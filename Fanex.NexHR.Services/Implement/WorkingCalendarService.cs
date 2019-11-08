namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;

    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly IWorkingCalendarRepository workingCalendarRepository;
        private readonly IBaseService baseService;

        public WorkingCalendarService(
            IWorkingCalendarRepository workingCalendarRepository,
            IBaseService baseService)
        {
            this.workingCalendarRepository = workingCalendarRepository;
            this.baseService = baseService;
        }

        public IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria)
        {
            IList<ShiftStatisticModel> shiftStatisticModels = baseService.GetData<ShiftStatisticModel>(criteria).ToList();

            return shiftStatisticModels ?? new List<ShiftStatisticModel>();
        }

        public IList<EmployeeShiftModel> GetEmployeeShift(GetEmployeeShiftDateCriteria criteria)
        {
            IList<EmployeeShiftModel> employeeShifts = workingCalendarRepository.GetEmployeeShift(criteria);

            return employeeShifts ?? new List<EmployeeShiftModel>();
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