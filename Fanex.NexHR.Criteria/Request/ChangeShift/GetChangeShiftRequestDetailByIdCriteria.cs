// --------------------------------------------------------------------------
// <copyright file="GetChangeShiftRequestDetailByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class GetChangeShiftRequestDetailByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}