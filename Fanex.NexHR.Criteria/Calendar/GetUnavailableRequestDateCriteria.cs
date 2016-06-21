// --------------------------------------------------------------------------
// <copyright file="GetUnavailableRequestDateCriteria.cs" company=".">
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
    /// Get Unavailable Request Date Criteria.
    /// </summary>
    public class GetUnavailableRequestDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetUnavailRequestDate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
