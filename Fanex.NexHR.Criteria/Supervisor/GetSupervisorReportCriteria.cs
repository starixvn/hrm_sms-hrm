// --------------------------------------------------------------------------
// <copyright file="GetSupervisorReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.17">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Supervisor Report Criteria.
    /// </summary>
    public class GetSupervisorReportCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int? ActionType { get; set; }

        public byte? IsLocal { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_Get";
        }
    }
}
