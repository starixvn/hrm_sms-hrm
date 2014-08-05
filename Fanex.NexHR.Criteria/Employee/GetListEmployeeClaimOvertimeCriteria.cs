// --------------------------------------------------------------------------
// <copyright file="GetListEmployeeClaimOvertimeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get List Employee Claim Overtime Criteria.
    /// </summary>
    public class GetListEmployeeClaimOvertimeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "OverTime_GetListEmpClaimOT";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
