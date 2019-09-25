namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class ResetChangeShiftRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_Reset";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}