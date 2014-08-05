// --------------------------------------------------------------------------
// <copyright file="DeleteRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    /// <summary>
    /// DeleteRequest Criteria.
    /// </summary>
    public class DeleteRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Delete";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
