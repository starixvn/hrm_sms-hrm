// --------------------------------------------------------------------------
// <copyright file="EmployeeShiftModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.22">Created</change>
// </history>

namespace Fanex.NexHR.Models.Calendar
{
    using System;

    public class EmployeeShiftModel
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public Shift Shift { get; set; }
    }
}
