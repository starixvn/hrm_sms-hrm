// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourWeeklyViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.1">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    /// <summary>
    /// TotalWorkingHourWeeklyViewModel ViewModel.
    /// </summary>
    public class TotalWorkingHourWeeklyViewModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }
    }
}
