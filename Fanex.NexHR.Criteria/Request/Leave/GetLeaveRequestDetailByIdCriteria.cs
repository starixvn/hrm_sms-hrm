namespace Fanex.NexHR.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "WorkingTimeRequest_GetLeaveRequestDetailByRequestID";

        public override bool IsValid() => RequestId > 0;
    }
}