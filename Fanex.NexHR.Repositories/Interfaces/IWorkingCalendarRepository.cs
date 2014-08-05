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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarRepository
    {
        IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(ICriteria criteria);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(ICriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(ICriteria criteria, DateTime date);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria);
    }
}
