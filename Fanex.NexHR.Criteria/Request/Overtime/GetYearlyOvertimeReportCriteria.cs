// -----------------------------------------------------------------------
// <copyright file="GetYearlyOvertimeReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.Overtime
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get yearly overtime report criteria.
    /// </summary>
    public class GetYearlyOvertimeReportCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public int? ExcludedEmpID { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestYearlyByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
