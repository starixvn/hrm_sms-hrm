namespace HRM.UserControl.Admin.Website.Criteria.Role
{
    public class UpdateRoleStatusCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public short RoleId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Role_UpdateStatus";

        public override bool IsValid() => SiteId > 0 && RoleId > 0 && ModifiedBy > 0;
    }
}