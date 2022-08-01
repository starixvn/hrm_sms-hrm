namespace HRM.UserControl.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class UserReCheckInCommand : NonQueryCommand
    {
        public long OnUserId { get; set; }
        public bool IsKickedOut { get; set; }

        public override string GetSettingKey() => "API_User_ReCheckIn";

        public override bool IsValid() => OnUserId > 0;
    }
}