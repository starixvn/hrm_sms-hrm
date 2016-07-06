namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetUnavailableRequestDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetUnavailRequestDate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
