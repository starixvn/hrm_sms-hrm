namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByCode";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Code);
        }
    }
}
