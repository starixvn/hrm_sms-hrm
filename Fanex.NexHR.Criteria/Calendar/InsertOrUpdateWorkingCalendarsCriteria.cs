// --------------------------------------------------------------------------
// <copyright file="InsertOrUpdateWorkingCalendarsCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert Or Update Working Calendars Criteria.
    /// </summary>
    public class InsertOrUpdateWorkingCalendarsCriteria : CriteriaBase
    {
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string ScheduleList { get; set; }

        public string ShiftMultipleList { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_Set_ShiftSchedule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
