namespace Fanex.NexHR.Criteria.EmployeeRelationship
{
    using Fanex.Data.Repository;

    public class CheckEmergencyExistsCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeRelationship_CheckEmergencyExist";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}