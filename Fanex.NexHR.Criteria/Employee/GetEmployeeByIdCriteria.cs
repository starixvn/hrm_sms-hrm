namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByID";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
