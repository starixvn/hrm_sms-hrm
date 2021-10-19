namespace HRM.UserControl.Admin.Website.Models.Session
{
    using System;

    [Serializable]
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}