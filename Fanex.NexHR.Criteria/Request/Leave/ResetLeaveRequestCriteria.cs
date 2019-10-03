namespace Fanex.NexHR.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class ResetLeaveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }

        public override string GetSettingKey() => "LeaveRequest_Reset";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}