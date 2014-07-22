// --------------------------------------------------------------------------
// <copyright file="GetTeamSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.22">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Team Supervisor Criteria.
    /// </summary>
    public class GetTeamSupervisorCriteria : CriteriaBase
    {
        public int TeamId { get; set; }

        public int SupervisorRole { get; set; }

        public override bool IsValid()
        {
            return TeamId > 0;
        }

        public override string GetSettingKey()
        {
            return string.Empty;
        }
    }
}
