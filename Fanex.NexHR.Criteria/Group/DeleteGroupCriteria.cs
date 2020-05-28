namespace Fanex.NexHR.Criteria.Group
{
    using Fanex.Data.Repository;

    public class DeleteGroupCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Delete";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}