// --------------------------------------------------------------------------
// <copyright file="UpdateShiftCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateShift Criteria.
    /// </summary>
    public class UpdateShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public string Name { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_Update";
        }

        public override bool IsValid()
        {
            return ShiftId >= 0 && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
