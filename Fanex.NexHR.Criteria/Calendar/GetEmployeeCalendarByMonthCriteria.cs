namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeCalendarByMonthCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int EmpId { get; set; }

        public string Groupids { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByMonth";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
