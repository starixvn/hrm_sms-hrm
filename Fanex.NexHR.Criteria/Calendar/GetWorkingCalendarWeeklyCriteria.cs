// --------------------------------------------------------------------------
// <copyright file="GetWorkingCalendarWeeklyCriteria.cs" company=".">
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
    /// Get Working Calendar Weekly Criteria.
    /// </summary>
    public class GetWorkingCalendarWeeklyCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public string JobTitleId { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByWeekly";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
