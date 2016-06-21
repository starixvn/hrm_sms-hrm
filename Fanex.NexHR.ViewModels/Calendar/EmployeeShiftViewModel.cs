// --------------------------------------------------------------------------
// <copyright file="EmployeeShiftViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeShiftViewModel
    {
        public DateTime WorkDate { get; set; }

        public int ShiftId { get; set; }

        public IList<Shift> Shifts { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
