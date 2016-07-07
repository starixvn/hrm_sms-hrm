namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    public class GetShiftStatisticCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public string GroupIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "ShiftMng_RptShiftSchedule";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
