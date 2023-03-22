namespace GO.Website.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class UserSiteMenu
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }

        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string SSOUrl { get; set; }
        public IList<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}