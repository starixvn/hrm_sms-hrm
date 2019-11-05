namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class EditDisciplineRuleCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        [SpParam(SqlParamName = "groupid")]
        public int SectionId { get; set; }

        public string DisciplineRuleXml { get; set; }

        public override bool IsValid() => SectionId > 0 && ActorId > 0;

        public override string GetSettingKey() => "DisciplineRule_Update";
    }
}