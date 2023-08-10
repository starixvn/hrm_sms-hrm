namespace GO.Website.Models.RoleAssignment
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string SiteName { get; set; }
        public string CompanyName { get; set; }

        public IList<UserRole> Roles { get; set; }
    }
}