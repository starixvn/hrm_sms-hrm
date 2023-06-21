namespace GO.Website.Models.Manpower.AnnualRequest
{
    using System.Collections.Generic;

    public class Section
    {
        public string SectionName { get; set; }
        public IEnumerable<string> JobLevels { get; set; }
    }
}
