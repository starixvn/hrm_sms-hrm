// --------------------------------------------------------------------------
// <copyright file="GetAllGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.27">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetGroupBySupervisorCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public override string GetSettingKey()
        {
            return "Group_GetBySupervisor";
        }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }
    }
}
