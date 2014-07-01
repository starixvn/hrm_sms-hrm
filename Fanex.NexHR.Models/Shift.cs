// --------------------------------------------------------------------------
// <copyright file="Shift.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// Shift Class.
    /// </summary>
    public class Shift : EntityBase<int>
    {
        public int ShiftID { get; set; }

        public string BeginCheckInTime { get; set; }

        public string EndCheckInTime { get; set; }

        public string BeginCheckOutTime { get; set; }

        public string EndCheckOutTime { get; set; }

        public int BreakTimeMins { get; set; }

        public string BeginBreakTime { get; set; }

        public string EndBreakTime { get; set; }

        public bool CrossDay { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsUsed { get; set; }
    }
}
