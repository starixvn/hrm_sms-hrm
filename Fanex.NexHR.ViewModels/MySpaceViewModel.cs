// --------------------------------------------------------------------------
// <copyright file="MySpaceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// MySpace ViewModel.
    /// </summary>
    public class MySpaceViewModel
    {
        public decimal AnnualLeaveBalance { get; set; }

        public decimal TotalWorkHours { get; set; }

        public decimal TotalWorkDays { get; set; }

        public decimal TotalPendingRequest { get; set; }

        public decimal TotalRequest { get; set; }

        public decimal TotalCheckInOutRequest { get; set; }

        public decimal TotalPendingCheckInOutRequest { get; set; }

        public decimal TotalLateInEarlyOutRequest { get; set; }

        public decimal TotalPendingLateInEarlyOutRequest { get; set; }

        public decimal TotalChangedShiftRequest { get; set; }

        public decimal TotalPendingChangedShiftRequest { get; set; }

        public decimal TotalLeaveRequest { get; set; }

        public decimal TotalPendingLeaveRequest { get; set; }

        public decimal TotalKIVRequest { get; set; }

        public decimal TotalPendingKIVRequest { get; set; }

        public decimal KeepInViewBalance { get; set; }

        public decimal OutStandingLeaveBalance { get; set; }

        public decimal OutStandingKIVLeaveBalance { get; set; }

        public decimal ForgotInOut { get; set; }

        public decimal LateInEarlyOut { get; set; }

        public decimal TotalPendingOverTimeRequest { get; set; }

        public IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> DateOfEmployeeForgotAndLateEarlyIO { get; set; }
    }
}