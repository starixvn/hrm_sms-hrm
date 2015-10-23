// --------------------------------------------------------------------------
// <copyright file="ShiftStatisticModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.26">Created</change>
// </history>

namespace Fanex.NexHR.Models.Calendar
{
    /// <summary>
    /// ShiftStatistic Model.
    /// </summary>
    public class ShiftStatisticModel
    {
        public int ShiftId { get; set; }

        public string ShiftCode { get; set; }

        public int WorkMonday { get; set; }

        public int WorkTuesday { get; set; }

        public int WorkWednesday { get; set; }

        public int WorkThursday { get; set; }

        public int WorkFriday { get; set; }

        public int WorkSaturday { get; set; }

        public int WorkSunday { get; set; }
    }
}
