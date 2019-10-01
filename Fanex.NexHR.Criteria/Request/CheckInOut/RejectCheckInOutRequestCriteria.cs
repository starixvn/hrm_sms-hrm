namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class RejectCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "CheckInOutRequest_Reject";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}