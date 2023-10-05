namespace GO.Website.Models.Account
{
    public class UserOtp
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public string SecretKey { get; set; }
        public string RecoveryCode { get; set; }
        public bool Enabled { get; set; }
        public bool IsRegistered { get; set; }
    }
}