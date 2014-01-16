// --------------------------------------------------------------------------
// <copyright file="ScheduleModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.21">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// Schedule Model.
    /// </summary>
    public class ScheduleModel
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public int? ShiftId { get; set; }

        public bool IsHoliday { get; set; }

        public bool IsOffDay { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsKeepInView { get; set; }

        public int LeaveTypeId { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public DateTime RequestCheckInTime { get; set; }

        public DateTime RequestCheckOutTime { get; set; }

        public int ChangeShiftId { get; set; }

        public decimal LeaveDay { get; set; }

        public bool IsPublic { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}
