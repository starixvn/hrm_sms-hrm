namespace GO.Admin.Website.Models.Site
{
    using System;

    [Serializable]
    public class Site
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string SSOUrl { get; set; }
        public string AccessToken { get; set; }
        public bool IsActive { get; set; }
        public int NumOfUser { get; set; }

        public short CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}