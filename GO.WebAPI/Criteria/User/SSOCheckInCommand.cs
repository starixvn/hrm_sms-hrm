namespace GO.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class SSOCheckInCommand : NonQueryCommand
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public string IP { get; set; }
        public string NetSessionId { get; set; }
        public long CheckOnUserId { get; set; }
        public long OnUserId { get; set; }

        public override string GetSettingKey() => "API_User_SSOCheckIn";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}