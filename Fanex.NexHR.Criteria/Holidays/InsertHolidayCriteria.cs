namespace Fanex.NexHR.Criteria.Holidays
{
    using Data.Repository;
    using System;

    public class InsertHolidayCriteria : CriteriaBase
    {
        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(HolidayName) && ModifiedBy > 0;
        }
    }
}
