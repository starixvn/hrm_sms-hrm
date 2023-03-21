namespace GO.AutoJobs.Models
{
    public class NexHrmApiConfig
    {
        public short NexHrmSiteId { get; set; }
        public short LumiHrmSiteId { get; set; }
        public string BaseUrl { get; set; }
        public string AuthenticateUrl { get; set; }
        public string AppCredentialId { get; set; }
        public string AppCredentialPassword { get; set; }

        public string EmployeeUrl { get; set; }
        public string SectionUrl { get; set; }
        public string TeamUrl { get; set; }
        public string JobTitleUrl { get; set; }
        public string UnitHeadInfoUrl { get; set; }
        public string UnitHeadScheduleUrl { get; set; }
    }
}