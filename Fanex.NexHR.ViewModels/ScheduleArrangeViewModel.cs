// --------------------------------------------------------------------------
// <copyright file="ScheduleArrangeViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;

    /// <summary>
    /// ScheduleArrange ViewModel.
    /// </summary>
    public class ScheduleArrangeViewModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public WorkingCalendarModel MondaySchedule { get; set; }

        public WorkingCalendarModel TuesdaySchedule { get; set; }

        public WorkingCalendarModel WednesdaySchedule { get; set; }

        public WorkingCalendarModel ThursdaySchedule { get; set; }

        public WorkingCalendarModel FridaySchedule { get; set; }

        public WorkingCalendarModel SaturdaySchedule { get; set; }

        public WorkingCalendarModel SundaySchedule { get; set; }
    }
}
