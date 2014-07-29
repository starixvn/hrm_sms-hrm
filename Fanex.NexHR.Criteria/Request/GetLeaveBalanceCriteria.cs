// --------------------------------------------------------------------------
// <copyright file="GetLeaveBalanceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetLeaveBalance Criteria.
    /// </summary>
    public class GetLeaveBalanceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public long? RequestId { get; set; }

        public DateTime? EffectedDay { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_AvaibleBalanceLeave";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
