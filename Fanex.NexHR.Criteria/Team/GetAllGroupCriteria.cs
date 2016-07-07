namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetGroupBySupervisorCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public override string GetSettingKey()
        {
            return "Group_GetBySupervisor";
        }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }
    }
}
