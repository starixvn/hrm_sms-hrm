namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyOvertimeRequestDetailsByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime Month { get; set; }

        public override string GetSettingKey() => "Overtime_GetMonthlyRequestDetailsByEmpId";

        public override bool IsValid() => EmployeeId > 0;
    }
}