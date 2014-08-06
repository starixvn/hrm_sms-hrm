// --------------------------------------------------------------------------
// <copyright file="GetTeamSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.22">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Team Supervisor Criteria.
    /// </summary>
    public class GetTeamSupervisorCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public int Role { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_GetByRole";
        }
    }
}
