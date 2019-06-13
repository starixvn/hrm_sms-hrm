namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetAllGroupCriteria : CriteriaBase
    {
        public bool IsDeleted { get; set; }

        public override string GetSettingKey() => "Group_Get";

        public override bool IsValid() => true;
    }
}