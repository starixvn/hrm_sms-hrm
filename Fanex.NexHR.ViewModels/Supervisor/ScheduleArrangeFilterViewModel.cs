namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ScheduleArrangeFilterViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Groups { get; set; }

        public IList<LookupData> JobTitles { get; set; }
    }
}