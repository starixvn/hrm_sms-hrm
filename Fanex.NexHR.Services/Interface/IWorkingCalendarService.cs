// --------------------------------------------------------------------------
// <copyright file="IWorkingCalendarService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarService
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(GetWorkingCalendarByDateRangeCriteria criteria);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(GetWorkingCalendarsForShiftArrangeCriteria criteria);

        CommandResult InsertOrUpdateWorkingCalendars(InsertOrUpdateWorkingCalendarsCriteria criteria);

        IList<ShiftStatisticModel> GetShiftStatistic(GetShiftStatisticCriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId, byte? isLocal = null);
    }
}
