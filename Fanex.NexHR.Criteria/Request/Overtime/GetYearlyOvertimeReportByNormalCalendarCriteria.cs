// --------------------------------------------------------------------------
// <copyright file="GetYearlyOvertimeReportByNormalCalendarCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.Overtime
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get yearly overtime by normal calendar criteria.
    /// </summary>
    public class GetYearlyOvertimeReportByNormalCalendarCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetNormalYearlyByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
