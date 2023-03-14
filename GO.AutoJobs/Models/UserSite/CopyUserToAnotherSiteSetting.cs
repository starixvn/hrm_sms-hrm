namespace GO.AutoJobs.Models.UserSite
{
    using System;

    [Serializable]
    public class CopyUserToAnotherSiteSetting
    {
        public short FromSiteId { get; set; }
        public short ToSiteId { get; set; }
    }
}