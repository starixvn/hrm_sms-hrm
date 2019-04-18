namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using System;
    using Fanex.Data.Repository;

    public class GetWeeklyScheduleCriteria : CriteriaBase
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int SupervisorId { get; set; }
        public int? EmpId { get; set; }
        public string GroupIds { get; set; }
        public bool IsOrderMode { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_GetWeekly";

        public override bool IsValid() => SupervisorId > 0 && ToTime.DayOfYear - FromTime.DayOfYear == 6;
    }
}