// --------------------------------------------------------------------------
// <copyright file="UpdateCheckInOutRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateCheckInOutRequest Criteria.
    /// </summary>
    public class UpdateCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public DateTime CheckTime { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public string Shift { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update_CheckInOutRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && (CheckTime - OnDate).Days >= 0 && (CheckTime - OnDate).Days <= 1;
        }
    }
}
