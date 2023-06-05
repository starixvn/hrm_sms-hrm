using Fanex.Data.Repository;

namespace GO.Website.Criteria.Manpower.Period
{
    public class GetPeriodCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public byte? IsActive { get; set; }
        public short PageIndex { get; set; }
        public short? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Get";

        public override bool IsValid() => true;
    }
}
