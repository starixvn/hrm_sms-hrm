using GO.Core.Models;

namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    public class UpdateAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public short PeriodId { get; set; }
        public string UnitName { get; set; }
        public string DetailXml { get; set; }
        public int RequestId { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateRequest";

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrWhiteSpace(UnitName) && RequestId > 0 && ModifiedBy > 0;
    }
}
