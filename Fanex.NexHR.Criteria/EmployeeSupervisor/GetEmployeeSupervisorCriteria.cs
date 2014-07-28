// --------------------------------------------------------------------------
// <copyright file="GetEmployeeSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.EmployeeSupervisor
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Supervisors Criteria.
    /// </summary>
    public class GetEmployeeSupervisorCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Role { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetRequestByEmpID";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
