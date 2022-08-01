namespace HRM.UserControl.Models
{
    using System;

    [Serializable]
    public class SiteModel
    {
        public short SiteId { get; set; }

        public string SiteName { get; set; }

        public string AccessToken { get; set; }

        public string SiteUrl { get; set; }

        public bool IsActive { get; set; }
    }
}