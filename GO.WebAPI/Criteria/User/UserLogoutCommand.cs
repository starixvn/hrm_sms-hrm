namespace GO.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class UserLogoutCommand : NonQueryCommand
    {
        public long OnUserId { get; set; }

        public override string GetSettingKey() => "API_User_Logout";

        public override bool IsValid() => OnUserId > 0;
    }
}