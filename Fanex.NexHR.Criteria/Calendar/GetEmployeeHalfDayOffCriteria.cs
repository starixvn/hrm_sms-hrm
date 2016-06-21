// --------------------------------------------------------------------------
// <copyright file="GetEmployeeHalfDayOffCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetEmployeeHalfDayOff Criteria.
    /// </summary>
    public class GetEmployeeHalfDayOffCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int? EmpId { get; set; }

        public int? Groupid { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetHalfDayOff";
        }

        public override bool IsValid()
        {
            return ReportDate != null;
        }
    }
}
