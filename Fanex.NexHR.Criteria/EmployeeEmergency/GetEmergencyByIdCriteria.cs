namespace Fanex.NexHR.Criteria.EmployeeEmergency
{
    using Fanex.Data.Repository;

    public class GetEmergencyByIdCriteria : CriteriaBase
    {
        public int EmergencyId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeEmergency_GetById";
        }

        public override bool IsValid()
        {
            return EmergencyId > 0;
        }
    }
}