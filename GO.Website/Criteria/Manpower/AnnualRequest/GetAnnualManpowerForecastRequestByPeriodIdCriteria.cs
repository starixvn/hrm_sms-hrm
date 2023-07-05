namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetAnnualManpowerForecastRequestByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetByPeriodId";

        public override bool IsValid() => PeriodId > 0 ;
    }
}