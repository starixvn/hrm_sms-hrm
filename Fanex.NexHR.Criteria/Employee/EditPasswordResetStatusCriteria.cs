namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class EditPasswordResetStatusCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public bool PasswordReset { get; set; }

        public override string GetSettingKey() => "Employee_UpdatePasswordResetStatusByEmpId";

        public override bool IsValid() => EmpId > 0;
    }
}