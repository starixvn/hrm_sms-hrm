namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class GetOtherViolationOfEmployeeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FromDate <= ToDate;
        }

        public override string GetSettingKey()
        {
            return "Discipline_GetOtherViolation";
        }
    }
}