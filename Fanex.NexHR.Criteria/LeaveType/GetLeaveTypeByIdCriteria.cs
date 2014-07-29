// --------------------------------------------------------------------------
// <copyright file="GetLeaveTypeByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>
namespace Fanex.NexHR.Criteria.LeaveType
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetLeaveTypeById Criteria.
    /// </summary>
    public class GetLeaveTypeByIdCriteria : CriteriaBase
    {
        public int LeaveTypeId { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveType_GetById";
        }

        public override bool IsValid()
        {
            return LeaveTypeId >= 0;
        }
    }
}
