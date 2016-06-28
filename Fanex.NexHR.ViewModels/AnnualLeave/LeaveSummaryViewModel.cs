namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }

        public int? Category { get; set; }
    }
}
