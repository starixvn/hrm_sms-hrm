namespace GO.WebAPI.Criteria.User
{
    using GO.Core.Models;

    public class DeleteUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "API_User_Delete";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}