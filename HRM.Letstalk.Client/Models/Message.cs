
namespace HRM.Letstalk.Client.Models
{
    using Newtonsoft.Json;
    using System;
    public class Message
    {
        [JsonProperty(PropertyName = "msg")]
        public string Content { get; set; }

        [JsonIgnore]
        public DateTime? ExpiredTime { get; set; }
    }
}
