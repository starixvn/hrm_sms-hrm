// --------------------------------------------------------------------------
// <copyright file="InsertShiftCriteria.cs" company=".">
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
    /// InsertShift Criteria.
    /// </summary>
    public class InsertShiftCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name); 
        }
    }
}
