namespace HRM.UserControl.Models.Auth
{
    public class LoginResult
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public bool IsLoginSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public string Token { get; set; }
    }
}