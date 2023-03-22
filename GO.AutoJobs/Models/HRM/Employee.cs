namespace GO.AutoJobs.Models
{
    using System;
    using GO.Core.Extensions.XML;
    using Newtonsoft.Json;

    [Serializable]
    [XMLName("Row")]
    public class Employee
    {
        [JsonProperty("SourceEmpId")]
        public int EmpId { get; set; }

        public string DomainUsername { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public short? JobTitleId { get; set; }
        public short? JobLevelId { get; set; }

        [JsonProperty("GroupId")]
        public int? TeamId { get; set; }

        [JsonProperty("Enabled")]
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string Email { get; set; }

        public string JoinedDate { get; set; }
        public string ConfirmedDate { get; set; }
        public string ResignedDate { get; set; }
        public string ModifiedDate { get; set; }
    }
}