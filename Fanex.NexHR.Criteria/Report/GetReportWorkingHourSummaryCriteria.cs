namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetReportWorkingHourSummaryCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Report_WorkingHoursSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
