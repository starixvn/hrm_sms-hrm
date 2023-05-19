namespace GO.Website.Models.Report.Manpower
{
    using System;

    [Serializable]
    public class EmployeeInfo
    {
        public string CompanyName { get; set; }
        public string SiteName { get; set; }
        public string Country { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public bool IsLocal { get; set; }
        public string JobLevel { get; set; }

        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}