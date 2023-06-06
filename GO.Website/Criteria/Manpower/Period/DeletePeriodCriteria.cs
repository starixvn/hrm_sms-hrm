namespace GO.Website.Criteria.Manpower.Period
{
    using GO.Core.Models;

    public class DeletePeriodCriteria : BaseNonQueryCommand
    {
        public int PeriodId { get; set; }
        public int SiteId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Delete";

        public override bool IsValid() => SiteId > 0 && PeriodId > 0 && ModifiedBy > 0;
    }
}
