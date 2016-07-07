namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkingCalendarByDateRangeCriteria : CriteriaBase
    {
        public int? SupervisorId { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetByDateRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
