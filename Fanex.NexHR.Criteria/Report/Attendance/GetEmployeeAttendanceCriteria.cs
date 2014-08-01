// --------------------------------------------------------------------------
// <copyright file="GetEmployeeAttendanceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report.Attendance
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Attendance Criteria.
    /// </summary>
    public class GetEmployeeAttendanceCriteria : CriteriaBase
    {
        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public int? EmpId { get; set; }

        public string Groupids { get; set; }

        public int? ExcludeUserDelete { get; set; }

        public byte? IsLocal { get; set; }

        public short Type { get; set; }

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
