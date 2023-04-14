namespace GO.Letstalk.WebAPI.Models
{
    using System;
    using Newtonsoft.Json;

    public class Message
    {
        [JsonProperty(PropertyName = "msg")]
        public string Content { get; set; }

        [JsonIgnore]
        public DateTime? ExpiredTime { get; set; }
    }
}