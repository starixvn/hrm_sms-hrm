// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByIdCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee By Employee Id Criteria.
    /// </summary>
    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int Empid { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByID";
        }

        public override bool IsValid()
        {
            return Empid > 0;
        }
    }
}
