namespace HRM.UserControl.Models.Auth
{
    public class LoginParam
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string Ip { get; set; }

        public string BrowserName { get; set; }

        public string Domain { get; set; }

        public string Platform { get; set; }
    }
}