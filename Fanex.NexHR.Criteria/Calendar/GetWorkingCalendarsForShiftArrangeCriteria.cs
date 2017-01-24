namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkingCalendarsForShiftArrangeCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public int? EmpId { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public string JobTitleId { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_GetShiftSchedule";
        }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }
    }
}
