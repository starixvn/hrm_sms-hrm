// --------------------------------------------------------------------------
// <copyright file="GetAllTeamSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.GroupSupervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get All Team Supervisor Criteria.
    /// </summary>
    public class GetAllTeamSupervisorCriteria : CriteriaBase
    {
        public string GroupIds { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_Get";
        }
    }
}
