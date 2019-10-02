namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using Fanex.Data.Repository;

    public class VoidKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "KeepInViewRequest_VoidBySA";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}