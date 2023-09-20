namespace GO.Website.Criteria.Manpower.AnnualRequest
{
    using Fanex.Data.Repository;

    public class GetMemberAnnualManpowerForecastRequestSummaryCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public short PeriodId { get; set; }
        public string UnitType { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetMemberSummaryRequest";

        public override bool IsValid() => PeriodId > 0;
    }
}
