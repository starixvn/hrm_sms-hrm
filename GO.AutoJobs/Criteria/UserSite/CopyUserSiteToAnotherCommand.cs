namespace GO.AutoJobs.Criteria.UserSite
{
    using HRM.UserControl.Core.Models;

    public class CopyUserSiteToAnotherCommand : BaseNonQueryCommand
    {
        public short FromSiteId { get; set; }
        public short ToSiteId { get; set; }

        public override string GetSettingKey() => "AutoJob_UserSite_CopyToAnotherSite";

        public override bool IsValid() => FromSiteId > 0 && ToSiteId > 0;
    }
}