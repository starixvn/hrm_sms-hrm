namespace HRM.UserControl.Models.Auth
{
    public class LoginResult
    {
        public int UserId { get; set; }
        public bool IsSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
        public JwtToken JwtToken { get; set; }
        public long OnUserId { get; set; }
    }
}