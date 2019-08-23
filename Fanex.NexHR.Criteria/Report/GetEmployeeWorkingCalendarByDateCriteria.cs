namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeWorkingCalendarByDateCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupIds { get; set; }
        public string Employee { get; set; }
        public byte? IsLocal { get; set; }
        public byte? EmployeeStatusId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Report_EmployeeCalendarByDateRange";
    }
}
