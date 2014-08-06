// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// AnnualLeaveBalance ViewModel.
    /// </summary>
    public class AnnualLeaveBalanceViewModel
    {
        public int EmployeeId { get; set; }

        public int Year { get; set; }

        [Display(Name = "Include employee's request")]
        public bool ExcludedRequestTrans { get; set; }
    }
}
