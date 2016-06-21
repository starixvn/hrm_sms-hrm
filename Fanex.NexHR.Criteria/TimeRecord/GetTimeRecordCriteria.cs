// --------------------------------------------------------------------------
// <copyright file="GetTimeRecordCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.TimeRecord
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Time Record Criteria.
    /// </summary>
    public class GetTimeRecordCriteria : CriteriaBase
    {
        public int? EmpID { get; set; }

        public byte? IsLocal { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_Get";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
