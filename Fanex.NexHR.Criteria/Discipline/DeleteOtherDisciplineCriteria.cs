namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteOtherDisciplineCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public int DisciplineTypeId { get; set; }

        public DateTime WorkDate { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && DisciplineTypeId != 0 && ModifiedBy > 0 && WorkDate != default(DateTime);
        }

        public override string GetSettingKey()
        {
            return "EmployeeDiscipline_DeleteOtherDiscipline";
        }
    }
}