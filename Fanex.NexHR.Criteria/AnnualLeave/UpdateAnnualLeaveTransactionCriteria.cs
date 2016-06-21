// --------------------------------------------------------------------------
// <copyright file="UpdateAnnualLeaveTransactionCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateAnnualLeaveTransaction Criteria.
    /// </summary>
    public class UpdateAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public long LeaveTransId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_UpdateTransaction";
        }

        public override bool IsValid()
        {
            return LeaveTransId > 0;
        }
    }
}
