namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using GO.Core.Models;

    public class UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommand : BaseNonQueryCommand
    {
        public int PeriodId { get; set; }
        public string UnitType { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateStatusByPeriodId";

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;
    }
}
