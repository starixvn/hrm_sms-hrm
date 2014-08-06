// --------------------------------------------------------------------------
// <copyright file="RequestsMonitoringViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// RequestMonitoring ViewModel.
    /// </summary>
    public class RequestsMonitoringViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Total Request >=")]
        public int MaxRequest { get; set; }

        public int DateOption { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }
    }
}
