namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetAllByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}