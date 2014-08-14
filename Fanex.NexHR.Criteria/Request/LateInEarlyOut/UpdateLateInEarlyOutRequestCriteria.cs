// --------------------------------------------------------------------------
// <copyright file="UpdateLateInEarlyOutRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateLateInEarlyOutRequest Criteria.
    /// </summary>
    public class UpdateLateInEarlyOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update_LateInEarlyOutRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && RequestId > 0;
        }
    }
}
