namespace HRM.UserControl.Client.Core.Models.Auth
{
    public class LoginResult
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public bool IsSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}