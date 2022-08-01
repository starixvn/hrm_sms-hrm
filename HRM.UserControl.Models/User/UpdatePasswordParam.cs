namespace HRM.UserControl.Models.User
{
    using System;

    [Serializable]
    public class UpdatePasswordParam
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}