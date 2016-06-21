// --------------------------------------------------------------------------
// <copyright file="DeleteShiftCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    /// <summary>
    /// DeleteShift Criteria.
    /// </summary>
    public class DeleteShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_Delete";
        }

        public override bool IsValid()
        {
            return ShiftId > 0;
        }
    }
}
