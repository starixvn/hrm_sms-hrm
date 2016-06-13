namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    public class GetOffDateAndHolidayDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetOffDate_HolidayDate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
