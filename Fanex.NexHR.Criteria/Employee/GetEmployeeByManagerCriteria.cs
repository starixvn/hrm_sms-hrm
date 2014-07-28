// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByManagerCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee By Manager Criteria.
    /// </summary>
    public class GetEmployeeByManagerCriteria : CriteriaBase
    {
        public int ManagerID { get; set; }

        public int Role { get; set; }

        public bool ExcludedManager { get; set; }

        public int? GroupID { get; set; }

        public string Keyword { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetMembesByManager";
        }

        public override bool IsValid()
        {
            return ManagerID > 0;
        }
    }
}
