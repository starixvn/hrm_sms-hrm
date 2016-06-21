namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetDisciplineSettingByEmployeeId : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetDisciplineSettingById";
        }
    }
}