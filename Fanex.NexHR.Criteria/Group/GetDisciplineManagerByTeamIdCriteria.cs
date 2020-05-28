namespace Fanex.NexHR.Group
{
    using Fanex.Data.Repository;

    public class GetDisciplineManagerByTeamIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override bool IsValid() => GroupId > 0;

        public override string GetSettingKey() => "GroupSupervisor_GetDisciplineSettingById";
    }
}