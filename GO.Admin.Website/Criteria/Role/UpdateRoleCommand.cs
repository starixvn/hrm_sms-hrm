namespace GO.Admin.Website.Criteria.Role
{
    public class UpdateRoleCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public short RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Role_Update";

        public override bool IsValid() => RoleId > 0 && SiteId > 0 && !string.IsNullOrWhiteSpace(Name) && ModifiedBy > 0;
    }
}