namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using GO.Core.Models;

    public class UpdateStatusMultiAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public string RequestIds { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateStatusMultiRequest";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(RequestIds) && ModifiedBy > 0;
    }
}
