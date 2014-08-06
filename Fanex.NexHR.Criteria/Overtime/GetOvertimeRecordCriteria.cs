// -----------------------------------------------------------------------
// <copyright file="GetOvertimeRecordCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    /// Get overtime record criteria.
    /// </summary>
    public class GetOvertimeRecordCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpID")]
        public int? EmployeeID { get; set; }

        public DateTime Month { get; set; }

        public bool IsDebug { get; set; }

        [SpParam(Ignored = true)]
        public int Calendar { get; set; }

        public override string GetSettingKey()
        {
            return Calendar == 1 ? "ExportExcel_OvertimeFiscal" : "ExportExcel_OvertimeNormal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
