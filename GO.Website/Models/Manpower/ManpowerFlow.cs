namespace GO.Website.Models.Manpower
{
    using System;
    using System.Xml.Serialization;
    using GO.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class ManpowerFlow
    {
        [XmlIgnore]
        public string UnitName { get; set; }

        public byte ProcessId { get; set; }
        public short SiteId { get; set; }

        [XmlIgnore]
        public string SiteName { get; set; }

        [XmlIgnore]
        public short CompanyId { get; set; }

        [XmlIgnore]
        public string CompanyName { get; set; }

        public int EmpId { get; set; }

        [XmlIgnore]
        public string Code { get; set; }

        [XmlIgnore]
        public string Nickname { get; set; }

        [XmlIgnore]
        public string FullName { get; set; }

        [XmlIgnore]
        public bool IsActive { get; set; }

        [XmlIgnore]
        public bool IsDeleted { get; set; }
    }
}