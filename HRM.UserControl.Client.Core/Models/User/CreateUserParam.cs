namespace HRM.UserControl.Client.Core.Models.User
{
    using System;

    [Serializable]
    public class CreateUserParam
    {
        public int? RefId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BCryptPassword { get; set; }
        public string Token { get; set; }
    }
}