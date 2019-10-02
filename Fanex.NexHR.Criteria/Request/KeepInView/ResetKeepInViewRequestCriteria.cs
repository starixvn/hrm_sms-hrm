namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using Fanex.Data.Repository;

    public class ResetKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }

        public override string GetSettingKey() => "KeepInViewRequest_Reset";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}