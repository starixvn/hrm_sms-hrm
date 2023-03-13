namespace HRM.Letstalk.Client.Models.Result
{
    using Newtonsoft.Json;

    public class WebhookResult : BaseResult
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
