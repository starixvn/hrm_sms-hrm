namespace Fanex.NexHR.ViewModels.Calendar
{
    using Models;
    using Models.Calendar;
    using System;
    using System.Collections.Generic;

    public class WorkingCalendarDetailViewModel
    {
        public WorkingCalendarDetailViewModel()
        {
            WorkingCalendars = new List<WorkingCalendarsViewModel>();
        }

        public int Year { get; set; }

        public int Month { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<WorkingCalendarsViewModel> WorkingCalendars { get; set; }
    }
}
