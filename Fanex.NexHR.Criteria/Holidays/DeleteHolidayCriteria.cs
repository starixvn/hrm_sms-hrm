namespace Fanex.NexHR.Criteria.Holidays
{
    using Data.Repository;
    using System;

    public class DeleteHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0 && ModifiedBy > 0;
        }
    }
}