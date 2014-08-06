// --------------------------------------------------------------------------
// <copyright file="InsertTeamShiftManagerCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert Team Shift Manager Criteria.
    /// </summary>
    public class InsertTeamShiftManagerCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string ShiftArrangers { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_UpdateForShiftArrange";
        }
    }
}
