namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDailyOvertimeReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? EmployeeId { get; set; }
        public string GroupIds { get; set; }
        public byte? IsLocal { get; set; }

        public override string GetSettingKey() => "Report_DailyOvertime";

        public override bool IsValid() => true;
    }
}