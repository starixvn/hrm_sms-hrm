namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class TotalWorkingHourWeeklyViewModel
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public byte? IsLocal { get; set; }
    }
}