namespace GO.Admin.Website.Criteria.User
{
    public class UpdateUserSiteRoleCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string RoleIds { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "User_UpdateSiteRole";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}