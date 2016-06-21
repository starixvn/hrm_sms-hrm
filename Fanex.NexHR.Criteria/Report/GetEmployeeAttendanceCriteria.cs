// --------------------------------------------------------------------------
// <copyright file="GetEmployeeAttendanceCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Attendance Criteria.
    /// </summary>
    public class GetEmployeeAttendanceCriteria : CriteriaBase
    {
        public int? SupervisorId { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public int? EmpId { get; set; }

        public string Groupids { get; set; }

        public int? ExcludeUserDelete { get; set; }

        public byte? IsLocal { get; set; }

        [SpParam(SqlParamName = "Type")]
        public short AttendanceType { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeAttendance_GetByType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
