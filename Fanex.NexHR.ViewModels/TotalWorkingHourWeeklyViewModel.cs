// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourWeeklyViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.1">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

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
