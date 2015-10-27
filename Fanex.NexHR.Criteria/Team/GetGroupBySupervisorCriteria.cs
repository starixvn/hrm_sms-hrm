// -----------------------------------------------------------------------
// <copyright file="GetAllGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.29">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get all group criteria.
    /// </summary>
    public class GetAllGroupCriteria : CriteriaBase
    {
        public bool IsDeleted { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Select";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
