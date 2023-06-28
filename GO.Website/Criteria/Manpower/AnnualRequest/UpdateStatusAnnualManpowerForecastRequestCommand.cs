namespace GO.Website.Criteria.Manpower
{
    using GO.Core.Models;

    public class UpdateStatusAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public int RequestId { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateStatus";

        public override bool IsValid() => RequestId > 0 && ModifiedBy > 0;
    }
}