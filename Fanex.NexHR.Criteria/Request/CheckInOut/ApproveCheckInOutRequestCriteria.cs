namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class ApproveCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }
        public string DisciplineXml { get; set; }

        public override string GetSettingKey() => "CheckInOutRequest_Approve";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}