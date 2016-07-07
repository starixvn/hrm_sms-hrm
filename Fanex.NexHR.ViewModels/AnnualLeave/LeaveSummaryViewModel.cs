namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public IList<Team> Teams { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }

        public int? Category { get; set; }

        public bool IsShowCloseBalance { get; set; }
    }
}
