namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyWorkingHoursCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupIds { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsLocal { get; set; }

        public override string GetSettingKey() => "Report_MonthlyWorkingHours";

        public override bool IsValid() => true;
    }
}