namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodByIdCriteria : CriteriaBase
    {
        public short PeriodId { get; set; }

        public override string GetSettingKey() => "Manpower_Period_GetById";

        public override bool IsValid() => PeriodId > 0;
    }
}