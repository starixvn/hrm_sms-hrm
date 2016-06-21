// --------------------------------------------------------------------------
// <copyright file="EmployeeAttendanceOvertime.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using Fanex.NexHR.Models.Overtime;

    /// <summary>
    /// EmployeeAttendanceOvertime Model.
    /// </summary>
    public class EmployeeAttendanceOvertime
    {
        public string OvertimeName { get; set; }

        public OvertimeTypeModel OvertimeType { get; set; }

        public DateTime? OvertimeFromTime { get; set; }

        public DateTime? OvertimeToTime { get; set; }

        public decimal? OvertimeTotalHours { get; set; }
    }
}
