namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using Fanex.Data.Repository;

    public class GetAllAnnualManpowerForecastRequestByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string UnitType { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetAllByPeriodId";

        public override bool IsValid() => PeriodId > 0;
    }
}
