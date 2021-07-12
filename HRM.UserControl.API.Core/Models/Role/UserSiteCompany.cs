namespace HRM.UserControl.API.Core.Models.Role
{
    public class UserSiteCompany
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string SiteURL { get; set; }

        public string AccessToken { get; set; }

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
    }
}