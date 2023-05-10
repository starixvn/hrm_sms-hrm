namespace GO.AutoJobs.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetTeamLastModifiedDateBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "AutoJob_Team_GetLastModifiedDateBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}