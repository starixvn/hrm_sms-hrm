namespace GO.Website.Criteria.Manpower
{
    using GO.Core.Models;

    public class CreateAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public short PeriodId { get; set; }
        public string DetailXml { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_Create";

        public override bool IsValid() => PeriodId > 0 && CreatedBy > 0;
    }
}