namespace Fanex.NexHR.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelCriteria : CriteriaBase
    {
        public int jobLevelId { get; set; }

        public override string GetSettingKey()
        {
            return "JobLevel_Delete";
        }

        public override bool IsValid()
        {
            return jobLevelId > 0;
        }
    }
}
