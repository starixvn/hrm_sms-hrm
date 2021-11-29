namespace HRM.UserControl.Admin.Website.Criteria.Role
{
    public class CreateRoleCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public short RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Role_Insert";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(Name) && CreatedBy > 0;
    }
}