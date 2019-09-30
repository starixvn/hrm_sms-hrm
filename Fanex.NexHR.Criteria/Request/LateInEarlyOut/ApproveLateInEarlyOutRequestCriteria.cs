namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class ApproveLateInEarlyOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }
        public string DisciplineXml { get; set; }

        public override string GetSettingKey() => "LateInEarlyOutRequest_Approve";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}