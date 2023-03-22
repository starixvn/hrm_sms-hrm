namespace GO.Admin.Website.Models.User
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class UserSite
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}