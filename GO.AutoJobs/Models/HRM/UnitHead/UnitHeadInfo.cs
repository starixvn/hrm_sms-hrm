namespace GO.AutoJobs.Models.UnitHead
{
    using System;
    using System.Xml.Serialization;
    using HRM.UserControl.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class UnitHeadInfo
    {
        public short CompanyId { get; set; }

        [XmlIgnore]
        public string CompanyName { get; set; }

        public short SiteId { get; set; }

        [XmlIgnore]
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