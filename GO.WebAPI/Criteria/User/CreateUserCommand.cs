namespace GO.WebAPI.Criteria.User
{
    using Fanex.Data;
    using GO.Core.Models;

    public class CreateUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        [SpParam(SqlParamName = "OldPassword")]
        public string Password { get; set; }

        [SpParam(SqlParamName = "Password")]
        public string BCryptPassword { get; set; }

        public int CreatedBy { get; set; }
        public int? RefId { get; set; }

        [SpParam(Ignored = true)]
        public string Token { get; set; }

        public override string GetSettingKey() => "API_User_Insert";

        public override bool IsValid() => SiteId > 0 && CreatedBy > 0;
    }
}