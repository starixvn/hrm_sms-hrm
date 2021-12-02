namespace HRM.UserControl.API.Core.Models
{
    public class AuthenticationSettings
    {
        public string SecretKey { get; set; }

        public int Expiration { get; set; }
    }
}