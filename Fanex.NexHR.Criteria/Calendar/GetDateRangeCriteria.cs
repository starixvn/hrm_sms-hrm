// --------------------------------------------------------------------------
// <copyright file="GetDateRangeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    /// GetDateRange Criteria.
    /// </summary>
    public class GetDateRangeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetRangeDate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
