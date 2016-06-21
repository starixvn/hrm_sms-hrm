// --------------------------------------------------------------------------
// <copyright file="InsertEmployeeCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert Employee Criteria.
    /// </summary>
    public class InsertEmployeeCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DomainUserName { get; set; }

        public string Code { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        public int MainGroupID { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreateBy { get; set; }

        public string Password { get; set; }

        public bool IsDefaultSchedule { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Information_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
