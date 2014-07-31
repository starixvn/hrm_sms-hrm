// -----------------------------------------------------------------------
// <copyright file="GetOvertimeMonthlyByHRCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get overtime monthly of employee by HR criteria.
    /// </summary>
    public class GetOvertimeMonthlyByHRCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "EmpID")]
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetOTMonthlyOfEmpByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0 && EmployeeId > 0;
        }
    }
}
