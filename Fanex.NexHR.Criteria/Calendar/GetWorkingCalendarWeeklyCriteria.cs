namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkingCalendarWeeklyCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public string JobTitleId { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByWeekly";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
