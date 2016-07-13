namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class WorkingCalendarSummaryViewModel
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }

        public string JobTitleId { get; set; }

        public IEnumerable<DateTime> UnavailDates { get; set; }
    }
}