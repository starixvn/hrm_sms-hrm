namespace HRM.UserControl.Client.Core.Models.Auth
{
    using System;

    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ValidTo;
    }
}