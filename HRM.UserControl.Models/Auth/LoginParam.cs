namespace HRM.UserControl.Models.Auth
{
    using System;

    [Serializable]
    public class LoginParam
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string IP { get; set; }
        public string BrowserName { get; set; }
        public string Domain { get; set; }
        public string Platform { get; set; }
        public string NetSessionId { get; set; }
    }
}