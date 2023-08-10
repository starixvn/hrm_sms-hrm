namespace GO.Website.Models.RoleAssignment
{
    using System;

    [Serializable]
    public class UserRole
    {
        public int UserId { get; set; }
        public short RoleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}