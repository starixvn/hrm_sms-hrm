namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetSectionSupervisorByRoleIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }
        public int Role { get; set; }

        public override bool IsValid() => GroupId > 0;

        public override string GetSettingKey() => "GroupSupervisor_GetByRole";
    }
}