// --------------------------------------------------------------------------
// <copyright file="ShiftDetails.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.15">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Shift Details Model.
    /// </summary>
    public class ShiftDetails
    {
        public DateTime OnDate { get; set; }

        public string FromShift { get; set; }

        public int FromShiftID { get; set; }

        public int ToShiftID { get; set; }

        public string Remark { get; set; }

        public string ToShift { get; set; }
    }
}
