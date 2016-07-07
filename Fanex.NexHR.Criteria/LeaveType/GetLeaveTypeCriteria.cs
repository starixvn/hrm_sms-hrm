namespace Fanex.NexHR.Criteria.LeaveType
{
    using Fanex.Data.Repository;

    public class GetLeaveTypeCriteria : CriteriaBase
    {
        public bool ExcludeNotShown { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveType_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
