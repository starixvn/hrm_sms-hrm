namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeDailyReportCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime Month { get; set; }

        public bool IsNormal { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetDailyReport";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}