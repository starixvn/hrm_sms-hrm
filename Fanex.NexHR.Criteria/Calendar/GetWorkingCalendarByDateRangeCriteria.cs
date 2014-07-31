// --------------------------------------------------------------------------
// <copyright file="GetWorkingCalendarByDateRangeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetWorkingCalendarByDateRange Criteria.
    /// </summary>
    public class GetWorkingCalendarByDateRangeCriteria : CriteriaBase
    {
        public int? SupervisorId { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetByDateRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
