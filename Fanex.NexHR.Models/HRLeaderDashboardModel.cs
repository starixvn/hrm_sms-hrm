namespace Fanex.NexHR.Models
{
    using System.Diagnostics.CodeAnalysis;

    public class HRLeaderDashboardModel
    {
        public int TotalPendingCheckInOutRequest { get; set; }

        public int TotalApprovedCheckInOutRequest { get; set; }

        public int TotalRejectCheckInOutRequest { get; set; }

        public int TotalPendingLateInEarlyOutRequest { get; set; }

        public int TotalApprovedLateInEarlyOutRequest { get; set; }

        public int TotalRejectLateInEarlyOutRequest { get; set; }

        public int TotalPendingChangeShiftRequest { get; set; }

        public int TotalApprovedChangeShiftRequest { get; set; }

        public int TotalRejectChangeShiftRequest { get; set; }

        public int TotalPendingLeaveRequest { get; set; }

        public int TotalApprovedLeaveRequest { get; set; }

        public int TotalRejectLeaveRequest { get; set; }

        public int TotalPendingKivRequest { get; set; }

        public int TotalApprovedKivRequest { get; set; }

        public int TotalRejecteKivRequest { get; set; }

        public int TotalPendingOvertime { get; set; }

        public int RoleId { get; set; }
    }
}
