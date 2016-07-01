namespace Fanex.NexHR.Criteria.Holidays
{
    using Data.Repository;
    using System;

    public class UpdateHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Update";
        }

        public override bool IsValid()
        {
            return Id > 0 && !string.IsNullOrEmpty(HolidayName) && ModifiedBy > 0;
        }
    }
}
