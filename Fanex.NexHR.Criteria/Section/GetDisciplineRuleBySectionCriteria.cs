namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetDisciplineRuleBySectionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "groupid")]
        public int SectionId { get; set; }

        public override string GetSettingKey() => "DisciplineRule_GetByGroupId";

        public override bool IsValid() => SectionId > 0;
    }
}