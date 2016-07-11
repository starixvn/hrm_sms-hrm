namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }

        public int? Category { get; set; }

        public bool IsShowCloseBalance { get; set; }
    }
}
