namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetYearlyOvertimeReportCriteria : CriteriaBase
    {
        public string GroupIds { get; set; }
        public int Year { get; set; }
        public byte CalendarType { get; set; }
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; }

        public override string GetSettingKey() => "Report_YearlyOvertime";

        public override bool IsValid() => true;
    }
}