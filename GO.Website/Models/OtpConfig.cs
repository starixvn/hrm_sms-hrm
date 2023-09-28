namespace GO.Website.Models
{
    public class OtpConfig
    {
        public string AppName { get; set; }

        public int OtpExpireTime { get; set; }
        public bool IsOtpRegistered { get; set; }
    }
}
