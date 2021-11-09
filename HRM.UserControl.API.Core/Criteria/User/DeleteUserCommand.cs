namespace HRM.UserControl.API.Core.Criteria.User
{
    using HRM.UserControl.Core.Models;

    public class DeleteUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "API_User_Delete";

        public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}