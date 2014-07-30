// --------------------------------------------------------------------------
// <copyright file="InsertAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Vietnam">
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

    /// <summary>
    /// InsertAnnualLeaveTransaction Criteria.
    /// </summary>
    public class InsertAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int RequestId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_InsertTransaction";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
