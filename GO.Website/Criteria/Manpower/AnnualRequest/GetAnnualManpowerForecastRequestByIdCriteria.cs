namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetAnnualManpowerForecastRequestByIdCriteria : CriteriaBase
    {
        public int RequestId { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_UpdateStatusMemberSummaryRequest";

        public override bool IsValid() => RequestId > 0;
    }
}