namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }
        public IList<Fanex.NexHR.Models.Team> Sections { get; set; }
        public string TeamIds { get; set; }
        public string Employee { get; set; }
        public int EmployeeId { get; set; }
        public int MinAnnualBalance { get; set; }
        public int? Category { get; set; }
        public UnpaidLeaveType LeaveType { get; set; }
        public bool IsShowCloseBalance { get; set; }

        public IEnumerable<LeaveSummaryByTeam> LeaveSumaries { get; set; }
    }
}