// --------------------------------------------------------------------------
// <copyright file="DeleteEmployeeCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete Employee Criteria.
    /// </summary>
    public class DeleteEmployeeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime ResignedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Delete";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
