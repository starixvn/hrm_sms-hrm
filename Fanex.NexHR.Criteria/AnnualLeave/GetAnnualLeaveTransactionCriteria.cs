// --------------------------------------------------------------------------
// <copyright file="GetAnnualLeaveTransactionCriteria.cs" company=".">
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
    /// GetAnnualLeaveTransaction Criteria.
    /// </summary>
    public class GetAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmpID { get; set; }

        public int Year { get; set; }

        public bool ExcludedRequestTrans { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetTransByEmp";
        }

        public override bool IsValid()
        {
            return EmpID > 0;
        }
    }
}
