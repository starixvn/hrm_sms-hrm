namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetPreviousApprovedManpowerForecastByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "ManpowerForecast_GetPreviousApprovedRequest";

        public override bool IsValid() => PeriodId > 0 ;
    }
}