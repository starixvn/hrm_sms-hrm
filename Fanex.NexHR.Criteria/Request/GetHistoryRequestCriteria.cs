// --------------------------------------------------------------------------
// <copyright file="GetHistoryRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetHistoryRequest Criteria.
    /// </summary>
    public class GetHistoryRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
