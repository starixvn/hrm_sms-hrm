namespace Fanex.NexHR.Criteria.Holidays
{
    using Data.Repository;

    public class GetHolidayByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_GetById"; 
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
