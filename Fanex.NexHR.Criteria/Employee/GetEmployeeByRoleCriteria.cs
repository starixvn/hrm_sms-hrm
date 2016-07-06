namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByRoleCriteria : CriteriaBase
    {
        public string RoleIds { get; set; }

        public string Query { get; set; }

        public string ExcludeIds { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetByRole";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
