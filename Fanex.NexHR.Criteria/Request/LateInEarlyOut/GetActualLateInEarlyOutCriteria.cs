// --------------------------------------------------------------------------
// <copyright file="GetActualLateInEarlyOutCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.27">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetActualLateInEarlyOutCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeSheet_GetLateInEarlyOut";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
