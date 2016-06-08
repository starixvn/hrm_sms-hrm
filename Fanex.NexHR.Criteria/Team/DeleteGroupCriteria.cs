namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteGroupCriteria : CriteriaBase
    {
        public int GroupID { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Delete";
        }

        public override bool IsValid()
        {
            return GroupID > 0;
        }
    }
}
