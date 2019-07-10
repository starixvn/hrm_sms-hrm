namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetYearlyOvertimeReportCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }
        public string GroupIds { get; set; }
        public int Year { get; set; }
        public byte CalendarType { get; set; }

        public override string GetSettingKey() => "Report_YearlyOvertime";

        public override bool IsValid() => true;
    }
}