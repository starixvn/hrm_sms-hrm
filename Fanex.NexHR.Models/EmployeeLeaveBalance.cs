// --------------------------------------------------------------------------
// <copyright file="EmployeeLeaveBalance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.4">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Employee Annual and Keep in view Balance.
    /// </summary>
    public class EmployeeLeaveBalance
    {
        public int EmployeeId { get; set; }

        public decimal AnnualBalance { get; set; }

        public decimal KeepInViewBalance { get; set; }

        public decimal CurrentLeaveBalanceDay { get; set; }
    }
}
