namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetDataForCreateAnnualManpowerForecastRequestCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public string UnitName { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetDataForCreate";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(UnitName);
    }
}