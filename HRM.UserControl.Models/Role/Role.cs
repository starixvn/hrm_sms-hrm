namespace HRM.UserControl.Models.Role
{
    public class Role
    {
        public short RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}