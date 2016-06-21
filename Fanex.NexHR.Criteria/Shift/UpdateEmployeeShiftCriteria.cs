// --------------------------------------------------------------------------
// <copyright file="UpdateEmployeeShiftCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Employee Shift Criteria.
    /// </summary>
    public class UpdateEmployeeShiftCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeShift_Update";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
