namespace HRM.Letstalk.Client.Models
{
    using HRM.Letstalk.Client.Enum;
    using Newtonsoft.Json;
    public class OutgoingMessage
    {
        [JsonIgnore]
        public MessageTypeNumber MessageType { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get
            {
                switch (MessageType)
                {
                    case MessageTypeNumber.Text:
                        return MulticastType.Text;

                    case MessageTypeNumber.Image:
                        return MulticastType.Image;

                    case MessageTypeNumber.FullImage:
                        return MulticastType.FullImage;

                    case MessageTypeNumber.Html:
                        return MulticastType.HTML;

                    default:
                        return MulticastType.Text;
                }
            }
        }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }
    }

    internal static class MulticastType
    {
        public const string Text = "text";
        public const string HTML = "html";
        public const string Image = "image";
        public const string FullImage = "fullimage";
    }
}
