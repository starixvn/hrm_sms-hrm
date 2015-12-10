// --------------------------------------------------------------------------
// <copyright file="GetEmployeeLateInEarlyOutCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeLateInEarlyOutCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "ReminderAttendance_GetLateInEarlyOut";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
