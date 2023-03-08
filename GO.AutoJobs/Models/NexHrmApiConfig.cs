namespace GO.AutoJobs.Models
{
    public class NexHrmApiConfig
    {
        public short SiteId { get; set; }
        public string BaseUrl { get; set; }
        public string AuthenticateUrl { get; set; }
        public string AppCredentialId { get; set; }
        public string AppCredentialPassword { get; set; }

        public string UnitHeadInfoUrl { get; set; }
        public string UnitHeadScheduleUrl { get; set; }
    }
}