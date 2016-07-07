namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetTotalWorkingHourMonthlyCriteria : CriteriaBase
    {
        public DateTime ReportMonth { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeTotalWorkingHourByMonth";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}