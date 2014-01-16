// --------------------------------------------------------------------------
// <copyright file="LeaveDetailModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.5">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// LeaveDetail Model.
    /// </summary>
    public class LeaveDetailModel
    {
        public DateTime LeaveDate { get; set; }

        public string ScheduleStatus { get; set; }

        public int WorkType { get; set; }

        public int LeaveTypeId { get; set; }

        public bool IsHalfDay { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public bool IsOffOrHoliday { get; set; }
    }
}
