namespace HRM.UserControl.Admin.Website.Models.User
{
    using System;

    [Serializable]
    public class UserRole
    {
        public short SiteId { get; set; }
        public short RoleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}