// --------------------------------------------------------------------------
// <copyright file="GetReportWorkingHourSummaryCriteria.cs" company=".">
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
    /// Get Report Working Hour Summary Criteria.
    /// </summary>
    public class GetReportWorkingHourSummaryCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Report_WorkingHoursSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
