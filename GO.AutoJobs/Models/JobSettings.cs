namespace GO.AutoJobs.Models
{
    using System;

    [Serializable]
    public class JobSettings
    {
        public short SiteId { get; set; }
        public string DbName { get; set; }
        public string JobName { get; set; }
        public string CronExpression { get; set; }
        public bool IsEnabled { get; set; }
    }
}