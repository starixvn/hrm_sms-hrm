namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    public class LeaveSummaryByTeam
    {
        public Team Team { get; set; }

        public List<LeaveSummary> LeaveSummaryInTeam { get; set; }
    }
}
