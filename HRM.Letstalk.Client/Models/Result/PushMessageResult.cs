using Newtonsoft.Json;
using System.Collections.Generic;

namespace HRM.Letstalk.Client.Models.Result
{
    public class PushMessageResult : BaseResult
    {
        [JsonProperty(PropertyName = "failList")]
        public List<string> FailList { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "blockList")]
        public List<string> BlockList { get; set; } = new List<string>();
    }
}
