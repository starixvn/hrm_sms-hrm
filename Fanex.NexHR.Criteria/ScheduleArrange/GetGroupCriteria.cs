namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using Fanex.Data.Repository;

    public class GetGroupCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_GetGroup";

        public override bool IsValid() => ActorId > 0;
    }
}