namespace Fanex.NexHR.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetEmployeeLoginByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override string GetSettingKey() => "Employee_GetEmployeeLoginByCode";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Code);
    }
}