// --------------------------------------------------------------------------
// <copyright file="GetUnpaidLeaveCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class GetUnpaidLeaveCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public byte? IsLocal { get; set; }

        public int? SupervisorId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_UnpaidLeaveSummary";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
