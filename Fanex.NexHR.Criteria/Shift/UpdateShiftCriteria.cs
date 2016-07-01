namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public string Name { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_Update";
        }

        public override bool IsValid()
        {
            return ShiftId >= 0 && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
