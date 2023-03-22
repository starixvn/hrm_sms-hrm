namespace GO.WebAPI.Criteria.User
{
    using GO.Core.Models;

    public class UpdateUserInfoCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }
        public int? RefId { get; set; }

        public override string GetSettingKey() => "API_User_Update";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && ModifiedBy > 0;
    }
}