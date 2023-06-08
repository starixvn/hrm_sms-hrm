using Fanex.Data.Repository;

namespace GO.Website.Criteria.Manpower.Period
{
    public class GetPeriodCriteriaByIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Manpower_Period_GetById";

        public override bool IsValid() => PeriodId > 0;
    }
}
