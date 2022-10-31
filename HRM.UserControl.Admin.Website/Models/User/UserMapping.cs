namespace HRM.UserControl.Admin.Website.Models.User
{
    using System;

    [Serializable]
    public class UserMapping
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public short SiteId { get; set; }
        public string SiteName { get; set; }
    }
}