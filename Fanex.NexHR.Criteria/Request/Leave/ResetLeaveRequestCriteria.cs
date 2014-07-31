// --------------------------------------------------------------------------
// <copyright file="ResetLeaveRequestCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// ResetLeaveRequest Criteria.
    /// </summary>
    public class ResetLeaveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        [SpParam(Ignored = true)]
        public string Reason { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_ResetRequest_byMember";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
