namespace Fanex.NexHR.Criteria.Group
{
    using Fanex.Data.Repository;

    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public bool? IsDeleted { get; set; }

        public override string GetSettingKey() => "Group_Get";

        public override bool IsValid() => GroupId > 0;
    }
}
