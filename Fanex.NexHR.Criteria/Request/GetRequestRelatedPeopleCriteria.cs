// --------------------------------------------------------------------------
// <copyright file="GetRequestRelatedPeopleCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetRequestRelatedPeople Criteria.
    /// </summary>
    public class GetRequestRelatedPeopleCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int Action { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetListSupervisorRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
