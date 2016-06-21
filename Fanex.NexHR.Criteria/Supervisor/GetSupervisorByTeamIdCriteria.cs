namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetSupervisorByTeamIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_GetById";
        }
    }
}
