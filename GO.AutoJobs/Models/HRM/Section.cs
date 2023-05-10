namespace GO.AutoJobs.Models.HRM
{
    using System;
    using GO.Core.Extensions.XML;
    using Newtonsoft.Json;

    [Serializable]
    [XMLName("Row")]
    public class Section
    {
        [JsonProperty("DepartmentId")]
        public int SectionId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        [JsonProperty("ParentId")]
        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string ModifiedDate { get; set; }
    }
}