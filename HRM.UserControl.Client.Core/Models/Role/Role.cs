namespace HRM.UserControl.Client.Core.Models.Role
{
    using System;

    [Serializable]
    public class Role
    {
        public short RoleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }

        public short SiteId { get; set; }
        public string SiteName { get; set; }
    }
}