// --------------------------------------------------------------------------
// <copyright file="UpdateSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.EmployeeSupervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Supervisor Criteria.
    /// </summary>
    public class UpdateSupervisorCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public short Role { get; set; }

        public string EmpIdLeaves { get; set; }

        public string EmpIdChangeShifts { get; set; }

        public string EmpIdLateInEarlyOuts { get; set; }

        public string EmpIdCheckInOuts { get; set; }

        public string EmpIdOvertimes { get; set; }

        public string EmpIdKeepInViews { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisorUpdate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
