// --------------------------------------------------------------------------
// <copyright file="TimesheetViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Timesheet ViewModel.
    /// </summary>
    public class TimesheetViewModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int? EmpId { get; set; }

        [Display(Name = "Request type")]
        public short TimeSheetType { get; set; }
    }
}
