namespace GO.Website.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetJobLevelSummaryReportCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Report_JobLevelSummary_Get";

        public override bool IsValid() => true;
    }
}