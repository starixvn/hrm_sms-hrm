namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using GO.Core.Models;

    public class UpdateStatusMemberSummaryRequestAnnualManpowerForecastRequestCommand : BaseNonQueryCommand
    {
        public string RequestId { get; set; }
        public byte StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateStatusByPeriodId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(RequestId) && ModifiedBy > 0;
    }
}
