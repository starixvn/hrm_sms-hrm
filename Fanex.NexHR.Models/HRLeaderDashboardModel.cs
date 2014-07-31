// --------------------------------------------------------------------------
// <copyright file="HRLeaderDashboardModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// HumanResourceLeaderDashboard Model.
    /// </summary>
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

        public int TotalPendingKIVRequest { get; set; }

        public int TotalApprovedKIVRequest { get; set; }

        public int TotalRejecteKIVRequest { get; set; }

        public int TotalPendingOvertime { get; set; }

        public int RoleId { get; set; }
    }
}
