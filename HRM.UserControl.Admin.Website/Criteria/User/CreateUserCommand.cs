using Fanex.Data;

namespace HRM.UserControl.Admin.Website.Criteria.User
{
    public class CreateUserCommand : BaseNonQueryCommand
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }

        [SpParam(Ignored = true)]
        public string Token { get; set; }

        public override string GetSettingKey() => "User_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && CreatedBy > 0;
    }
}