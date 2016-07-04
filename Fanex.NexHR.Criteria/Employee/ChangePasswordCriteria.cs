namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class ChangePasswordCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string Password { get; set; }

        [SpParam(Ignored = true)]
        public string NewPassword { get; set; }

        public bool PasswordReset { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_ChangePassword";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
