namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using System;

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
