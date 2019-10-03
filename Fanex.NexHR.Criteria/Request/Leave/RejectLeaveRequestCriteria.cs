namespace Fanex.NexHR.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class RejectLeaveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "LeaveRequest_Reject";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}