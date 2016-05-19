namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetCheckInOutRequestToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetCheckInOutRequestToIgnoreById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}