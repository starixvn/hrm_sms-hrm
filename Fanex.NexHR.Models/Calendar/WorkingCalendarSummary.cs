// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarSummary.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Models.Calendar
{
    using System;

    /// <summary>
    /// Working Calendar Summary.
    /// </summary>
    public class WorkingCalendarSummary
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Schedule { get; set; }

        public DateTime WorkDate { get; set; }
    }
}
