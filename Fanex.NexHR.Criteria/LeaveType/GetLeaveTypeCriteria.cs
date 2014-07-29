// --------------------------------------------------------------------------
// <copyright file="GetLeaveTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.LeaveType
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetLeaveType Criteria.
    /// </summary>
    public class GetLeaveTypeCriteria : CriteriaBase
    {
        public bool ExcludeNotShown { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveType_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
