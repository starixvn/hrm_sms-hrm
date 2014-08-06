// -----------------------------------------------------------------------
// <copyright file="GetMonthlyOvertimeReportByNormalCalendarCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get monthly overtime report by normal calendar criteria.
    /// </summary>
    public class GetMonthlyOvertimeReportByNormalCalendarCriteria : CriteriaBase
    {
        public int? SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetNormalMonthlyByHR";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
