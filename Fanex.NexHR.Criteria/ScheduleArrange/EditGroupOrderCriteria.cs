namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using Fanex.Data.Repository;

    public class EditGroupOrderCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string DisplayOrderXml { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_UpdateGroupOrder";

        public override bool IsValid() => ActorId > 0;
    }
}