namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class DeleteRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "WorkingTimeRequest_Delete";

        public override bool IsValid() => RequestId > 0;
    }
}