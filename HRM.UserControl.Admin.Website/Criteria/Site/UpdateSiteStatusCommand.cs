namespace HRM.UserControl.Admin.Website.Criteria.Site
{
    public class UpdateSiteStatusCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Site_UpdateStatus";

        public override bool IsValid() => SiteId > 0 && ModifiedBy > 0;
    }
}