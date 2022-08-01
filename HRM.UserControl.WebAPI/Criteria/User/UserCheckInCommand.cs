namespace HRM.UserControl.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class UserCheckInCommand : NonQueryCommand
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public string IP { get; set; }
        public string NetSessionId { get; set; }
        public long OnUserId { get; set; }

        public override string GetSettingKey() => "API_User_CheckIn";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}