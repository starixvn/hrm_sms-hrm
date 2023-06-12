namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodCriteria : CriteriaBase
    {
        public byte? IsActive { get; set; }
        public short PageIndex { get; set; }
        public short? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Get";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}
