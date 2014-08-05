// --------------------------------------------------------------------------
// <copyright file="GetShiftStatisticCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Shift Statistic Criteria.
    /// </summary>
    public class GetShiftStatisticCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_RptShiftSchedule";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
