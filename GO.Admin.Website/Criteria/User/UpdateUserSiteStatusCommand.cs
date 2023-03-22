namespace GO.Admin.Website.Criteria.User
{
    public class UpdateUserSiteStatusCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "User_UpdateSiteStatus";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}