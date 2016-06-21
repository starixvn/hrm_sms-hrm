// --------------------------------------------------------------------------
// <copyright file="ResetAnnualLeaveCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Reset Annual Leave Criteria.
    /// </summary>
    public class ResetAnnualLeaveCriteria : CriteriaBase
    {
        public string EmpValue { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal ResetDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_ResetMultipleTransactions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
