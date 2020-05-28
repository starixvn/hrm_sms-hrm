namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetTeamSupervisorCriteria : CriteriaBase
    {
        public string GroupIds { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_Get";
        }
    }
}
