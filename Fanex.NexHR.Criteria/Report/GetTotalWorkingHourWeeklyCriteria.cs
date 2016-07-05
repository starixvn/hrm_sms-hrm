namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetTotalWorkingHourWeeklyCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeTotalWorkingHourByWeek";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
