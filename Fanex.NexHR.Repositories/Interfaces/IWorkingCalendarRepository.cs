// --------------------------------------------------------------------------
// <copyright file="IWorkingCalendarRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.ViewModels.Calendar;

    public interface IWorkingCalendarRepository
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(ICriteria criteria);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(ICriteria criteria);

        IList<WorkingScheduleViewModel> GetWorkingSchedule(ICriteria criteria, DateTime workingDate);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria);
    }
}
