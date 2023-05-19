namespace GO.Website.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetManpowerReportCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Report_Manpower_Get";

        public override bool IsValid() => true;
    }
}