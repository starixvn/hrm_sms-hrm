// -----------------------------------------------------------------------
// <copyright file="GetDetailOvertimeByRequestIdsCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.8.1">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get detail overtime by request Id criteria.
    /// </summary>
    public class GetDetailOvertimeByRequestIdsCriteria : CriteriaBase
    {
        public string ListRequestID { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetDetailByListRequestID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
