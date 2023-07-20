namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using GO.Core.Models;

    public class UpdateAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public int RequestId { get; set; }
        public string DetailXml { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_Update";

        public override bool IsValid() => RequestId > 0 && ModifiedBy > 0;
    }
}