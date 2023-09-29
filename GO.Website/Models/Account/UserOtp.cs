namespace GO.Website.Models.Account
{
    public class UserOtp
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string SecretKey { get; set; }
        public bool Enabled { get; set; }
        public bool IsOTPRegistered { get; set; }
    }
}
