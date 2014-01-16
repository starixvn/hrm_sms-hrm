// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourMonthly.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.1.7">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Total Working Hour Monthly.
    /// </summary>
    public class TotalWorkingHourMonthly
    {
        public string StaffId { get; set; }

        public string Section { get; set; }

        public string FullNameNick { get; set; }

        public string Nick { get; set; }

        public decimal CountWorkDay { get; set; }

        public List<string> WorkingHours { get; set; }
    }
}
