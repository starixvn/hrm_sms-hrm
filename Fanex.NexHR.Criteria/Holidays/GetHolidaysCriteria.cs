namespace Fanex.NexHR.Criteria.Holidays
{
    using Data.Repository;

    public class GetHolidaysCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_GetByYear";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
