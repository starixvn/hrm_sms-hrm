namespace Fanex.NexHR.Criteria.Discipline
{
    using Data.Repository;

    public class UpdateIgnoredDisciplineDetailCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string IgnoreDetailXml { get; set; }

        public override string GetSettingKey() => "DisciplineIgnoreDetail_Update";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(IgnoreDetailXml);
    }
}