// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByRoleCriteria.cs" company=".">
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
    /// Get Employee By Role Criteria.
    /// </summary>
    public class GetEmployeeByRoleCriteria : CriteriaBase
    {
        public string RoleIds { get; set; }

        public string Query { get; set; }

        public string ExcludeIds { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetByRole";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
