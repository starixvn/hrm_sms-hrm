namespace GO.Website.Models
{
    public class JwtAuthSettings
    {
        public string SecretKey { get; set; }
        public short Expiration { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}