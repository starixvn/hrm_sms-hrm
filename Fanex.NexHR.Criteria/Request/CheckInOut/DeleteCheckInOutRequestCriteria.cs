namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class DeleteCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "KeepInViewRequest_Delete";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
