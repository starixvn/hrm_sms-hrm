// --------------------------------------------------------------------------
// <copyright file="GetEmployeeCalendarByMonthCriteria.cs" company=".">
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
    /// Get Employee Calendar By Month Criteria.
    /// </summary>
    public class GetEmployeeCalendarByMonthCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int EmpId { get; set; }

        public string Groupids { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByMonth";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
