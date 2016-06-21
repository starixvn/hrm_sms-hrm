// --------------------------------------------------------------------------
// <copyright file="TimeRecordUpdateTypeCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.TimeRecord
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TimeRecordUpdateType Criteria.
    /// </summary>
    public class TimeRecordUpdateTypeCriteria : CriteriaBase
    {
        public int RecordTimeId { get; set; }

        public byte InOutType { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_UpdateType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
