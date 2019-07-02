namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetMonthlyOvertimeRequestByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }

        public override string GetSettingKey() => "Overtime_GetMonthlyRequestByEmpId";

        public override bool IsValid() => EmployeeId > 0;
    }
}