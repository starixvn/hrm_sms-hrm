namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class ResetCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }

        public override string GetSettingKey() => "CheckInOutRequest_Reset";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}