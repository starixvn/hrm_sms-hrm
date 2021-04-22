namespace Fanex.NexHR.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class GetJobLevelCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "JobLevel_Get";

        public override bool IsValid() => true;
    }
}
