namespace GO.Website.Models.Manpower.AnnualRequest
{
    using System.Collections.Generic;

    public class Department
    {
        public string DepartmentName { get; set; }
        public IEnumerable<string> Sections { get; set; }
    }
}
