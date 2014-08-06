// -----------------------------------------------------------------------
// <copyright file="GetOvertimeReportInYearCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get overtime in year criteria.
    /// </summary>
    public class GetOvertimeReportInYearCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public int Year { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestMonthlyByEmpId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
