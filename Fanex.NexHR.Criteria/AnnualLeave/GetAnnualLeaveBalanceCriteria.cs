// --------------------------------------------------------------------------
// <copyright file="GetAnnualLeaveBalanceCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Annual Leave Balance Criteria.
    /// </summary>
    public class GetAnnualLeaveBalanceCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public byte? IsLocal { get; set; }

        public int? SupervisorID { get; set; }

        public string Role { get; set; }

        public override string GetSettingKey()
        {
            return "Report_LeaveSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
