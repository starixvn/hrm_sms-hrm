// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Calendar;

    /// <summary>
    /// WorkingCalendar Service.
    /// </summary>
    public class WorkingCalendarService : BaseService, IWorkingCalendarService
    {
        private readonly IWorkingCalendarRepository _workingCalendarRepository;

        public WorkingCalendarService()
        {
            this._workingCalendarRepository = new WorkingCalendarRepository();
        }

        public WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository)
        {
            this._workingCalendarRepository = workingCalendarRepository;
        }

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="criteria">GetWorkingCalendarByDateRange Criteria.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarModel> calendars = null;
            if (criteria.IsValid())
            {
                calendars = this._workingCalendarRepository.GetWorkingCalendarByDateRange(criteria);
            }

            return calendars;
        }

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(GetWorkingCalendarsForShiftArrangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarModel> workingCalendarModels = null;
            if (criteria.IsValid())
            {
                workingCalendarModels = _workingCalendarRepository.GetWorkingCalendarsForShiftArrange(criteria);
            }

            return workingCalendarModels;
        }

        public CommandResult InsertOrUpdateWorkingCalendars(InsertOrUpdateWorkingCalendarsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }
            
            return result;
        }

        /// <summary>
        /// Get Shift Statistic.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Shift Statistic Models.</returns>
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

        public IList<WorkingScheduleViewModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleViewModel> workingSchedule = null;
            if (criteria.IsValid())
            {
                workingSchedule = this._workingCalendarRepository.GetWorkingSchedule(criteria, criteria.ReportDate);
            }

            return workingSchedule;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendarList = null;
            if (criteria.IsValid())
            {
                workingCalendarList = this._workingCalendarRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendarList;
        }
    }
}
