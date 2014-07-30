// --------------------------------------------------------------------------
// <copyright file="GetUserChangedShiftCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get User Changed Shift Criteria.
    /// </summary>
    public class GetUserChangedShiftCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int? EmpId { get; set; }

        public int? Groupid { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetChangedShift";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
