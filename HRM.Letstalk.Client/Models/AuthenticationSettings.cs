namespace HRM.Letstalk.Client.Models
{
    public class AuthenticationSettings
    {
        public string AppCredentialId { get; set; }
        public string AppCredentialPassword { get; set; }
        public string SecretKey { get; set; }
        public string AuthenticateUrl { get; set; }
        public string SendMultipleMesssages { get; set; }
        public string SendSingleMessage { get; set; }
        public int Expiration { get; set; }

    }
}
