namespace GO.Letstalk.WebAPI.Models
{
    public class JwtAuthSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecretKey { get; set; }
        public int Expiration { get; set; }
    }
}