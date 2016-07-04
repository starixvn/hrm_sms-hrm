namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class UpdatePasswordResetStatusCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public bool PasswordReset { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_UpdatePasswordResetStatusByEmpId";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
