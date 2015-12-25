// --------------------------------------------------------------------------
// <copyright file="GetGroupByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get group by Id criteria.
    /// </summary>
    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public bool? IsDeleted { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Get";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}
