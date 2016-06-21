// --------------------------------------------------------------------------
// <copyright file="GetEmployeeShiftCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Shift Criteria.
    /// </summary>
    public class GetEmployeeShiftCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeShift_Get";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
