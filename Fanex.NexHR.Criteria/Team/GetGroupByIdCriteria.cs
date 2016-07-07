namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public bool? IsDeleted { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Get";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}
