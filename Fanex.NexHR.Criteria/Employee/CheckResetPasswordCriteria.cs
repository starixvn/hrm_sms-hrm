namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class CheckResetPasswordCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_CheckResetPassword";

        public override bool IsValid() => EmployeeId > 0;
    }
}