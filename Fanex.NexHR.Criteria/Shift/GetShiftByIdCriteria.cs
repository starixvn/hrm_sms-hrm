// --------------------------------------------------------------------------
// <copyright file="GetShiftByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetShiftById Criteria.
    /// </summary>
    public class GetShiftByIdCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_GetById";
        }

        public override bool IsValid()
        {
            return ShiftId >= 0;
        }
    }
}
