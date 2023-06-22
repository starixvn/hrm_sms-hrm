namespace GO.Website.Models.Manpower.AnnualRequest
{
    using System.Collections.Generic;

    public class Company
    {
        public string Name { get; set; }
        public IEnumerable<string> Units { get; set; }
        public IEnumerable<string> Departments { get; set; }
        public IEnumerable<string> Sections { get; set; }
    }
}
