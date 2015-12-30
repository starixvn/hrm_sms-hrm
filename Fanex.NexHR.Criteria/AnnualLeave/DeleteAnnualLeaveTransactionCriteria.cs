// --------------------------------------------------------------------------
// <copyright file="DeleteAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public long LeaveTransId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_DeleteTransaction";
        }

        public override bool IsValid()
        {
            return LeaveTransId > 0;
        }
    }
}
