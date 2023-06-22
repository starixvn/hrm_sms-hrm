namespace GO.Website.Models.Manpower.AnnualRequest
{
    using System.Collections.Generic;

    public class Unit
    {
        public string UnitName { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
