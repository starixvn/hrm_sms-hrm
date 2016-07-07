namespace Fanex.NexHR.Criteria.Calendar
{
    using Fanex.Data.Repository;

    public class GetEmployeeByShiftManagementCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetEmployeeByShiftManagement";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
