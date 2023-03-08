namespace GO.Website.Models.UnitHead
{
    using System;

    [Serializable]
    public class UnitHeadInfo
    {
        public short CompanyId { get; set; }
        public string CompanyName { get; set; }

        public short SiteId { get; set; }
        public string SiteName { get; set; }

        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }
}