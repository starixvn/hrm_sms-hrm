// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourDailyViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System;

    public class TotalWorkingHourDailyViewModel
    {
        public DateTime WorkDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public bool IsExportExcel { get; set; }

        public byte? IsLocal { get; set; }
    }
}
