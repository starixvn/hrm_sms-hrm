// -----------------------------------------------------------------------
// <copyright file="GetOvertimeExcelReportDetailCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.8.1">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get overtime excel report detail criteria.
    /// </summary>
    public class GetOvertimeExcelReportDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "EmpID")]
        public int? EmployeeID { get; set; }

        public string JobTitleId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_OvertimeDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
