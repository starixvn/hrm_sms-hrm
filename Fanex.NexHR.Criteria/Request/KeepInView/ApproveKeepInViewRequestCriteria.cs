namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using Fanex.Data.Repository;

    public class ApproveKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "KeepInViewRequest_Approve";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}