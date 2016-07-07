namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class InsertOrUpdateWorkingCalendarsCriteria : CriteriaBase
    {
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string ScheduleList { get; set; }

        public string ShiftMultipleList { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_Set_ShiftSchedule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
