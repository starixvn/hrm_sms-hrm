namespace HRM.UserControl.Models.Auth
{
    public class LoginParam
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public string IP { get; set; }
        public string BrowserName { get; set; }
        public string Domain { get; set; }
        public string Platform { get; set; }
    }
}