// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarSummaryViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.11">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Working Calendar Summary View Model.
    /// </summary>
    public class WorkingCalendarSummaryViewModel
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }

        public string JobTitleId { get; set; }

        public IEnumerable<DateTime> UnavailDates { get; set; }
    }
}
