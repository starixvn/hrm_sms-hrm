namespace HRM.UserControl.WebAPI.Models
{
    public class AuthenticationSettings
    {
        public string SecretKey { get; set; }

        public int Expiration { get; set; }
    }
}