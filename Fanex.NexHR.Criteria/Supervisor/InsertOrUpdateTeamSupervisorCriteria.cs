// --------------------------------------------------------------------------
// <copyright file="InsertOrUpdateTeamSupervisorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.23">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert Or Update Team Supervisor Criteria.
    /// </summary>
    public class InsertOrUpdateTeamSupervisorCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public int Role { get; set; }

        public string EmpIdLeaves { get; set; }

        public string EmpIdChangeShifts { get; set; }

        public string EmpIdLateInEarlyOuts { get; set; }

        public string EmpIdCheckInOuts { get; set; }

        public string EmpIdOvertimes { get; set; }

        public string EmpIdKeepInViews { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_Set";
        }
    }
}
