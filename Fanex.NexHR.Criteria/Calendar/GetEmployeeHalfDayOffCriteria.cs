namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeHalfDayOffCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int? EmpId { get; set; }

        public int? Groupid { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetHalfDayOff";
        }

        public override bool IsValid()
        {
            return ReportDate != null;
        }
    }
}
