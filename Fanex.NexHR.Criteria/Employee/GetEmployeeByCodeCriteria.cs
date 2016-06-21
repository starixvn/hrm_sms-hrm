// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByCodeCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee By Code Criteria.
    /// </summary>
    public class GetEmployeeByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByCode";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Code);
        }
    }
}
