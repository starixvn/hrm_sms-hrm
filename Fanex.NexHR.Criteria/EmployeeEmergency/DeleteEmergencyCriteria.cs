namespace Fanex.NexHR.Criteria.EmployeeEmergency
{
    using Fanex.Data.Repository;

    public class DeleteEmergencyCriteria : CriteriaBase
    {
        public int EmergencyId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeEmergency_Delete";
        }

        public override bool IsValid()
        {
            return EmergencyId > 0 && ActorId > 0;
        }
    }
}