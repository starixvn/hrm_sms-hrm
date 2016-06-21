// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceMultiViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// AnnualLeaveBalanceMulti ViewModel.
    /// </summary>
    public class AnnualLeaveBalanceMultiViewModel : BaseAnnualLeaveBalanceViewModel
    {
        [Required]
        public string EmpIds { get; set; }
    }
}
