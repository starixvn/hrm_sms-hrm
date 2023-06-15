namespace GO.Website.Criteria.Manpower.Period
{
    using GO.Core.Models;

    public class DeletePeriodCommand : BaseNonQueryCommand
    {
        public short PeriodId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Delete";

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;
    }
}