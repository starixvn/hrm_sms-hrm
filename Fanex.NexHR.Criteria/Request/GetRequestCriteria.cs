// --------------------------------------------------------------------------
// <copyright file="GetRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetRequest Criteria.
    /// </summary>
    public class GetRequestCriteria : CriteriaBase
    {
        public int? SupervisorId { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; }

        public string Role { get; set; }

        public bool? IsLocal { get; set; }

        public int? RequestType { get; set; }

        [SpParam(Ignored = true)]
        public bool IsQueryByCreatedDate { get; set; }

        public override string GetSettingKey()
        {
            return IsQueryByCreatedDate ? "WorkingTimeRequest_GetRequestByCreateDate_AllType" : "WorkingTimeRequest_GetRequestByRequestDate_AllType";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
