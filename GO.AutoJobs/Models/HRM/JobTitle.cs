namespace GO.AutoJobs.Models.HRM
{
    using System;
    using GO.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class JobTitle
    {
        public int JobTitleId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}