// --------------------------------------------------------------------------
// <copyright file="AdjustEmployeeAnnualLeaveCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    /// Adjust Employee Annual Leave Criteria.
    /// </summary>
    public class AdjustEmployeeAnnualLeaveCriteria : CriteriaBase
    {
        public string EmpIds { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_InsertMultipleTransactions";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmpIds);
        }
    }
}
