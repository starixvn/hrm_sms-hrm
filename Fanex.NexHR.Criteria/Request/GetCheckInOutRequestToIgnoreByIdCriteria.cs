namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetCheckInOutRequestToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "WorkingTimeRequest_GetCheckInOutRequestToIgnoreById";

        public override bool IsValid() => RequestId > 0;
    }
}