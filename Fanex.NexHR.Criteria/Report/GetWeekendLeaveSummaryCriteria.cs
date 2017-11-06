using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.Report
{
    public class GetWeekendLeaveSummaryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }

        public override string GetSettingKey()
        {
            return "Report_GetWeekendLeaveSummary";
        }
    }
}