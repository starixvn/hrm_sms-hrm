// --------------------------------------------------------------------------
// <copyright file="UpdateKeepInViewRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateKeepInViewRequest Criteria.
    /// </summary>
    public class UpdateKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public bool IsHalfDay { get; set; }

        public int ShiftId { get; set; }

        [SpParam(Ignored = true)]
        public string ShiftName { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update_KeepInViewRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
