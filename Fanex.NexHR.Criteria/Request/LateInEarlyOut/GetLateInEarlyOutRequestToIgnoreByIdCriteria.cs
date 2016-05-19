namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutRequestToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetLateInEarlyOutRequestToIgnoreById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}