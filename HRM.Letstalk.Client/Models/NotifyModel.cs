namespace HRM.Letstalk.Client.Models
{
    using System.Collections.Generic;

    public class NotifyModel
    {
        public int BotId { get; set; }
        public string ConversationId { get; set; }
        public List<string> ConversationIds{ get; set; }
        public string Message { get; set; }
    }
}
