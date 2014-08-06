// -----------------------------------------------------------------------
// <copyright file="GetOvertimeByRequestIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get overtime by request Id criteria.
    /// </summary>
    public class GetOvertimeByRequestIdCriteria : CriteriaBase
    {
        public long RequestID { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetRequestByRequestID";
        }

        public override bool IsValid()
        {
            return RequestID > 0;
        }
    }
}
