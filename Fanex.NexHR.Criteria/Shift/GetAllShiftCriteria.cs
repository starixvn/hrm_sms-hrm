// --------------------------------------------------------------------------
// <copyright file="GetAllShiftCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
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
    /// GetAllShift Criteria.
    /// </summary>
    public class GetAllShiftCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Shift_GetShifts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
