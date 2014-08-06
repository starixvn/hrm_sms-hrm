// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;

    /// <summary>
    /// AnnualLeaveBalance Model.
    /// </summary>
    public class AnnualLeaveBalanceModel : BaseAnnualLeaveBalanceViewModel
    {
        public long LeaveTransId { get; set; }

        public int EmpId { get; set; }

        public long? RequestId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
