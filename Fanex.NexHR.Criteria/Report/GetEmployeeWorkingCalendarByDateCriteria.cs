// --------------------------------------------------------------------------
// <copyright file="GetEmployeeWorkingCalendarByDateCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Working Calendar By Date Criteria.
    /// </summary>
    public class GetEmployeeWorkingCalendarByDateCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public string JobTitleId { get; set; }

        public byte? IsLocal { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByDateRange";
        }
    }
}
