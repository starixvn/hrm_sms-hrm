namespace HRM.UserControl.Admin.Website.Criteria.User
{
    public class UpdateUserMappingCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public int? NewUserId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "UserMapping_Update";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}