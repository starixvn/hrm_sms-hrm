namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class TotalWorkingHourDailyViewModel
    {
        public DateTime WorkDate { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }
    }
}