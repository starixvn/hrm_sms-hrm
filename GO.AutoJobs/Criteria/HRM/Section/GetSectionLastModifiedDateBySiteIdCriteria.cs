namespace GO.AutoJobs.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetSectionLastModifiedDateBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "AutoJob_Section_GetLastModifiedDateBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}