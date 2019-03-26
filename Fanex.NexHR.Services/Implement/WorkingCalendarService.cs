namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
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

        public IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarModel> calendars = null;
            if (criteria.IsValid())
            {
                calendars = workingCalendarRepository.GetWorkingCalendarByDateRange(criteria);
            }

            return calendars;
        }

        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(GetWorkingCalendarsForShiftArrangeCriteria criteria)
        {
            IList<WorkingCalendarModel> workingCalendarModels = null;

            if (criteria.IsValid())
            {
                workingCalendarModels = workingCalendarRepository.GetWorkingCalendarsForShiftArrange(criteria);
            }

            return workingCalendarModels ?? new List<WorkingCalendarModel>();
        }

        public IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria)
        {
            IList<ShiftStatisticModel> shiftStatisticModels = null;

            if (criteria.IsValid())
            {
                shiftStatisticModels = baseService.GetData<ShiftStatisticModel>(criteria).ToList();
            }

            return shiftStatisticModels ?? new List<ShiftStatisticModel>();
        }

        public IList<EmployeeShiftModel> GetEmployeeShift(GetEmployeeShiftDateCriteria criteria)
        {
            IList<EmployeeShiftModel> employeeShifts = null;

            if (criteria.IsValid())
            {
                employeeShifts = workingCalendarRepository.GetEmployeeShift(criteria);
            }

            return employeeShifts ?? new List<EmployeeShiftModel>();
        }

        public IList<WorkingScheduleViewModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleViewModel> workingSchedule = null;
            if (criteria.IsValid())
            {
                workingSchedule = workingCalendarRepository.GetWorkingSchedule(criteria, criteria.ReportDate);
            }

            return workingSchedule;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendarList = null;
            if (criteria.IsValid())
            {
                workingCalendarList = workingCalendarRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendarList;
        }

        public IList<SupervisorModel> GetEmployeeByShiftManagement(GetEmployeeByShiftManagementCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = baseService.GetData<SupervisorModel>(criteria).ToList();
            }

            return supervisors;
        }
    }
}