namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetMyAnnualManpowerForecastRequestCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetMyRequest";

        public override bool IsValid() => UserId > 0;
    }
}