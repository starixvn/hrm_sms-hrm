// --------------------------------------------------------------------------
// <copyright file="GetEmployeeSupervisorByQueryCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.EmployeeSupervisor
{
    using Fanex.Data.Repository;
    
    /// <summary>
    /// GetEmployeeSupervisorByQuery Criteria.
    /// </summary>
    public class GetEmployeeSupervisorByQueryCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public byte Role { get; set; }

        public byte? RequestType { get; set; }

        public string Query { get; set; }

        public bool IncludeLoginUser { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetByQuery";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
