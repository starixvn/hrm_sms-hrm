namespace HRM.UserControl.Client.Core.Models.User
{
    public class UpdateUserInfoParam
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}