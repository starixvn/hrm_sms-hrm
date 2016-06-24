namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class TimeRecordViewModel : PagerOptionModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public IList<Team> Teams { get; set; }

        public byte? IsLocal { get; set; }
    }
}