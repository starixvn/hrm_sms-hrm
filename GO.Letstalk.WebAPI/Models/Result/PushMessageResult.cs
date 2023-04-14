namespace GO.Letstalk.WebAPI.Models.Result
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PushMessageResult : BaseResult
    {
        [JsonProperty(PropertyName = "failList")]
        public List<string> FailList { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "blockList")]
        public List<string> BlockList { get; set; } = new List<string>();
    }
}