// --------------------------------------------------------------------------
// <copyright file="TimeRecordViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.1.27">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    /// <summary>
    /// TimeRecord ViewModel.
    /// </summary>
    public class TimeRecordViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public IList<Team> Teams { get; set; }

        public byte? IsLocal { get; set; }
    }
}
