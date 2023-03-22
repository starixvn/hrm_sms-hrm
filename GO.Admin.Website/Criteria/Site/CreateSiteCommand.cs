namespace GO.Admin.Website.Criteria.Site
{
    public class CreateSiteCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string SSOUrl { get; set; }
        public string AccessToken { get; set; }
        public short CompanyId { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Site_Insert";

        public override bool IsValid() => CompanyId > 0 && !string.IsNullOrWhiteSpace(SiteName) && CreatedBy > 0;
    }
}