namespace HRM.UserControl.Client.Core.Models.Auth
{
    using System;

    [Serializable]
    public class LoginResult
    {
        public long OnUserId { get; set; }
        public int UserId { get; set; }
        public bool IsSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
        public JwtToken JwtToken { get; set; }
    }
}