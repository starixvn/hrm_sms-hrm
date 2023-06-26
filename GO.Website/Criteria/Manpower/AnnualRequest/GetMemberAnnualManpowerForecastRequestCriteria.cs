namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetMemberAnnualManpowerForecastRequestCriteria : CriteriaBase
    {
        public short PeriodId { get; set; }
        public int UserId { get; set; }
        public byte? StatusId { get; set; }

        public override string GetSettingKey() => "Manpower_AnnualRequestForm_GetMemberRequest";

        public override bool IsValid() => PeriodId > 0 && UserId > 0;
    }
}