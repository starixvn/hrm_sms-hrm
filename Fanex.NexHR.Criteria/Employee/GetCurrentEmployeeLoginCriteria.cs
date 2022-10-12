namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetCurrentEmployeeLoginCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetCurrentEmployeeLogin";

        public override bool IsValid() => true;
    }
}
