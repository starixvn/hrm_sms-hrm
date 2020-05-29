namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    public class LeaveSummaryByTeam
    {
        public int GroupId { get; set; }
        public string Team { get; set; }

        public List<LeaveSummary> LeaveSummaryInTeam { get; set; }
    }
}
