namespace GO.WebAPI.Criteria.Site
{
    using Fanex.Data.Repository;

    public class GetSiteByIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "API_Site_GetById";

        public override bool IsValid() => SiteId > 0;
    }
}