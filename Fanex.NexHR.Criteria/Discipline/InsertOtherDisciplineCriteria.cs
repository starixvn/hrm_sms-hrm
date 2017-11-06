namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Data.Repository;

    public class InsertOtherDisciplineCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public int DisciplineTypeId { get; set; }

        public decimal Score { get; set; }

        [SpParam(SqlParamName = "ApproverComment")]
        public string Comment { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => EmployeeId > 0 && CreatedBy > 0 && Score > 0;

        public override string GetSettingKey() => "EmployeeDiscipline_AddOtherDiscipline";
    }
}