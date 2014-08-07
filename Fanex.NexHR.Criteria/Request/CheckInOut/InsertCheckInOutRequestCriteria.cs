// --------------------------------------------------------------------------
// <copyright file="InsertCheckInOutRequestCriteria.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Core.Attributes;

    /// <summary>
    /// InsertCheckInOutRequest Criteria.
    /// </summary>
    public class InsertCheckInOutRequestCriteria : CriteriaBase
    {
        [Output]
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public DateTime CheckTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public string Shift { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_CheckInOutRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && (CheckTime - OnDate).Days >= 0 && (CheckTime - OnDate).Days <= 1;
        }
    }
}
