// --------------------------------------------------------------------------
// <copyright file="GetAnnualLeaveTransactionByIdCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetAnnualLeaveTransactionById Criteria.
    /// </summary>
    public class GetAnnualLeaveTransactionByIdCriteria : CriteriaBase
    {
        public int LeaveTransId { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetByID";
        }

        public override bool IsValid()
        {
            return LeaveTransId > 0;
        }
    }
}
