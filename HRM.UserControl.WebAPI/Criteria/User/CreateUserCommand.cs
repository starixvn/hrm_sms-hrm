namespace HRM.UserControl.WebAPI.Criteria.User
{
    using Fanex.Data;
    using HRM.UserControl.Core.Models;

    public class CreateUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public int CreatedBy { get; set; }

        [SpParam(Ignored = true)]
        public string Token { get; set; }

        public override string GetSettingKey() => "API_User_Insert";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(Password) && CreatedBy > 0;
    }
}