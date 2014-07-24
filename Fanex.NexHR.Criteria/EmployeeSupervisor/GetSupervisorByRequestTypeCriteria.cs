// --------------------------------------------------------------------------
// <copyright file="GetSupervisorByRequestTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.EmployeeSupervisor
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetSupervisorByRequestType Criteria.
    /// </summary>
    public class GetSupervisorByRequestTypeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public byte Role { get; set; }

        public byte RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetSupervisorForRequest";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
