namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class InsertOtherViolationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public int DisciplineTypeId { get; set; }

        [SpParam(SqlParamName = "ApproverComment")]
        public string Comment { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && CreatedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeDiscipline_AddOtherViolation";
        }
    }
}