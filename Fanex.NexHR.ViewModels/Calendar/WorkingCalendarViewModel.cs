namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class WorkingCalendarViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public IEnumerable<WorkingCalendarRangeDates> WorkingCalendars { get; set; } = new List<WorkingCalendarRangeDates>();
    }
}