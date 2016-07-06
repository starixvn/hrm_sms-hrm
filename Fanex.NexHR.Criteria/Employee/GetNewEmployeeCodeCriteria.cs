namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetNewEmployeeCodeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetNewEmployeecode";
        }
    }
}
