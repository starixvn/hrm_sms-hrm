namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetLeaveRequestDetail";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}