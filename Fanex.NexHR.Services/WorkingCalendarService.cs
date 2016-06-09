namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Calendar;
    using System.Collections.Generic;
    using System.Linq;

    public class WorkingCalendarService : BaseService, IWorkingCalendarService
    {
        private readonly ICommandService commandService;
        private readonly IWorkingCalendarRepository workingCalendarRepository;

        public WorkingCalendarService() : this(null, null)
        {
        }

        public WorkingCalendarService(ICommandService commandService, IWorkingCalendarRepository workingCalendarRepository)
        {
            this.workingCalendarRepository = workingCalendarRepository ?? new WorkingCalendarRepository();
            this.commandService = commandService ?? new CommandService();
        }

        public WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository)
        {
            this.workingCalendarRepository = workingCalendarRepository;
        }

        public IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarModel> calendars = null;
            if (criteria.IsValid())
            {
                calendars = this.workingCalendarRepository.GetWorkingCalendarByDateRange(criteria);
            }

            return calendars;
        }

        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(GetWorkingCalendarsForShiftArrangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarModel> workingCalendarModels = null;
            if (criteria.IsValid())
            {
                workingCalendarModels = workingCalendarRepository.GetWorkingCalendarsForShiftArrange(criteria);
            }

            return workingCalendarModels;
        }

        public CommandResult InsertOrUpdateWorkingCalendars(InsertOrUpdateWorkingCalendarsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = commandService.Insert(criteria);
            }
            
            return result;
        }

        public IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<ShiftStatisticModel> shiftStatisticModels = null;
            if (criteria.IsValid())
            {
                shiftStatisticModels = this.GetData<ShiftStatisticModel>(criteria).ToList();
            }

            return shiftStatisticModels;
        }

        public IList<EmployeeShiftModel> GetEmployeeShift(GetEmployeeShiftDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EmployeeShiftModel> employeeShifts = null;
            if (criteria.IsValid())
            {
                employeeShifts = workingCalendarRepository.GetEmployeeShift(criteria);
            }

            return employeeShifts;
        }

        public IList<WorkingScheduleViewModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleViewModel> workingSchedule = null;
            if (criteria.IsValid())
            {
                workingSchedule = this.workingCalendarRepository.GetWorkingSchedule(criteria, criteria.ReportDate);
            }

            return workingSchedule;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendarList = null;
            if (criteria.IsValid())
            {
                workingCalendarList = this.workingCalendarRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendarList;
        }

        public IList<SupervisorModel> GetEmployeeByShiftManagement(GetEmployeeByShiftManagementCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = this.GetData<SupervisorModel>(criteria).ToList();
            }

            return supervisors;
        }
    }
}
