namespace HRM.UserControl.Client.Core.Models.User
{
    public class UpdatePasswordParam
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public short SiteId { get; set; }
        public int ModifiedBy { get; set; }
    }
}