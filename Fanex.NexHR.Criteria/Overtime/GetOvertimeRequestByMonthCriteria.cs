namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeRequestByMonthCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GeByMonth";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}