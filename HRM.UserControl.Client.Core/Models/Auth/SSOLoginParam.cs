namespace HRM.UserControl.Client.Core.Models.Auth
{
    using System;

    [Serializable]
    public class SSOLoginParam
    {
        public int UserId { get; set; }
        public long CheckOnUserId { get; set; }
        public long OnUserId { get; set; }
        public string IP { get; set; }
        public string NetSessionId { get; set; }
        public string BrowserName { get; set; }
        public string Domain { get; set; }
        public string Platform { get; set; }
    }
}