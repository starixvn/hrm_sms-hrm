// --------------------------------------------------------------------------
// <copyright file="GetOffDateAndHolidayDateCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Off Date And Holiday Date Criteria.
    /// </summary>
    public class GetOffDateAndHolidayDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetOffDate_HolidayDate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
