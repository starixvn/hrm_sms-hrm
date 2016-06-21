namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetDisciplineSettingByGroupIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_GetDisciplineSettingById";
        }
    }
}