namespace Fanex.NexHR.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestHistoryByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveRequest_GetHistoryById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
