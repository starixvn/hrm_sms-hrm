namespace Fanex.NexHR.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveRequest_GetById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}