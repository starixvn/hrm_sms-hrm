namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutRequestHistoryByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "LateInEarlyOutRequest_GetHistoryById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
