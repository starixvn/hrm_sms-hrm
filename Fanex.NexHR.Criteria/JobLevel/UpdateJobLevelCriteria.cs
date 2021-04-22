namespace Fanex.NexHR.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class UpdateJobLevelCriteria : CriteriaBase
    {
        public int JobLevelId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "JobLevel_Update";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && JobLevelId > 0;
        }
    }
}
