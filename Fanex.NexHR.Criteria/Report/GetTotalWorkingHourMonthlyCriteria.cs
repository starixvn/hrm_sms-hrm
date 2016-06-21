// --------------------------------------------------------------------------
// <copyright file="GetTotalWorkingHourMonthlyCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Total Working HourMonthlyCriteria.
    /// </summary>
    public class GetTotalWorkingHourMonthlyCriteria : CriteriaBase
    {
        public DateTime ReportMonth { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeTotalWorkingHourByMonth";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
