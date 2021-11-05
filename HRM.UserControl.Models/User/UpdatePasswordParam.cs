namespace HRM.UserControl.Models.User
{
    public class UpdatePasswordParam
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}