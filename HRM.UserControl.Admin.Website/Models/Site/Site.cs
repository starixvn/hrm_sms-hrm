namespace HRM.UserControl.Admin.Website.Models.Site
{
    public class Site
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string AccessToken { get; set; }
        public bool IsActive { get; set; }
        public int NumOfUser { get; set; }
    }
}