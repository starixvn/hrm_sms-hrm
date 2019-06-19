namespace Fanex.NexHR.Criteria.EmployeeEmergency
{
    using Fanex.Data.Repository;

    public class GetEmergencyByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeEmergency_GetByEmployeeId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}