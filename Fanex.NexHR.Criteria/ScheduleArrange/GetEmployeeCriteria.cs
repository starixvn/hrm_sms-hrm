namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using Fanex.Data.Repository;

    public class GetEmployeeCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string GroupIds { get; set; }
        public string Query { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_GetEmployee";

        public override bool IsValid() => ActorId > 0;
    }
}