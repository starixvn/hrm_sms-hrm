// -----------------------------------------------------------------------
// <copyright file="GetOvertimeMonthlyByEmployeeIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get overtime monthly by employee Id criteria.
    /// </summary>
    public class GetOvertimeMonthlyByEmployeeIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpID")]
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public string GroupIds { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetInOutMonthlyByEmpId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
