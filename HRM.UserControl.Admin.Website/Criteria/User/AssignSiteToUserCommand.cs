namespace HRM.UserControl.Admin.Website.Criteria.User
{
    public class AssignSiteToUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string RoleIds { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "User_AssignSite";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && CreatedBy > 0;
    }
}