namespace GO.Website.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetJobLevelCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_GetJobLevel";

        public override bool IsValid() => true;
    }
}
