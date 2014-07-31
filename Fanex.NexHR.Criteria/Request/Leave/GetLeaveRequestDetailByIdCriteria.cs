// --------------------------------------------------------------------------
// <copyright file="GetLeaveRequestDetailByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetLeaveRequestDetailById Criteria.
    /// </summary>
    public class GetLeaveRequestDetailByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetLeaveRequestDetailByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
