namespace HRM.UserControl.Models
{
    using System.Collections.Generic;

    public class SiteModel
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string AccessToken { get; set; }

        public string SiteUrl { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<Company> CompanyList { get; set; }
    }
}