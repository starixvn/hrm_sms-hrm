// -----------------------------------------------------------------------
// <copyright file="UpdateOvertimeRequestCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Update overtime request criteria.
    /// </summary>
    public class UpdateOvertimeRequestCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int? HRId { get; set; }

        public string ListDetail { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_SET_OTRequest";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
