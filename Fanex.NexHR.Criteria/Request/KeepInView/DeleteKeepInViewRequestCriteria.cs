namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using Fanex.Data.Repository;

    public class DeleteKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "CheckInOutRequest_Delete";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
