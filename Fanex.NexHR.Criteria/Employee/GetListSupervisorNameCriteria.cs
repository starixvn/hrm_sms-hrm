// --------------------------------------------------------------------------
// <copyright file="GetListSupervisorNameCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get List Supervisor Name Criteria.
    /// </summary>
    public class GetListSupervisorNameCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public byte Role { get; set; }

        public byte RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetListSupervisorByEmpID";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
