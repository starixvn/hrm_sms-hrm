
namespace HRM.Letstalk.Client.Models.Result
{
    using HRM.Letstalk.Client.Extentions.XML;

    [XMLName("r")]
    public class SendNotificationResult
    {
        public int NotificationId { get; set; }

        public int ConversationGroupId { get; set; }

        public string ErrMsg { get; set; }

        public string Status { get; set; }
    }
}
