namespace HRM.UserControl.Admin.Website.Criteria.Site
{
    public class UpdateSiteCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string AccessToken { get; set; }
        public short CompanyId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Site_Update";

        public override bool IsValid() => CompanyId > 0 && SiteId > 0 && !string.IsNullOrWhiteSpace(SiteName) && ModifiedBy > 0;
    }
}