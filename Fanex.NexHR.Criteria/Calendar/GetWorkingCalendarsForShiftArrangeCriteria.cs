// --------------------------------------------------------------------------
// <copyright file="GetWorkingCalendarsForShiftArrangeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    /// Get Working Calendars For Shift Arrange Criteria.
    /// </summary>
    public class GetWorkingCalendarsForShiftArrangeCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        public int? EmpID { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public string JobTitleId { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_GetShiftSchedule";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
