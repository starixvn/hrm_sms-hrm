// --------------------------------------------------------------------------
// <copyright file="HalfDayOff.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models.Calendar
{
    /// <summary>
    /// HalfDayOff Model.
    /// </summary>
    public class HalfDayOff
    {
        public int ID { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int Day { get; set; }

        public string Code { get; set; }
    }
}
