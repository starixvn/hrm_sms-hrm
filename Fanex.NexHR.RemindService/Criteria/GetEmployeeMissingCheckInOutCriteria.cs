// --------------------------------------------------------------------------
// <copyright file="GetEmployeeMissingCheckInOutCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeMissingCheckInOutCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "ReminderAttendance_GetMissingCheckinOut";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
