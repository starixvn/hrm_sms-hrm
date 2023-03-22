namespace GO.AutoJobs.Models
{
    using System;
    using GO.Core.Extensions.XML;
    using Newtonsoft.Json;

    [Serializable]
    [XMLName("Row")]
    public class Team
    {
        [JsonProperty("GroupId")]
        public int TeamId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        [JsonProperty("DepartmentId")]
        public int? SectionId { get; set; }

        public bool IsDeleted { get; set; }

        public string ModifiedDate { get; set; }
    }
}