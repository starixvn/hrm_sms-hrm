namespace GO.Website.Models.Manpower
{
    using System;
    using System.Xml.Serialization;
    using GO.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class AnnualManpowerForecastRequestDetail
    {
        public short SiteId { get; set; }

        [XmlIgnore]
        public string SiteName { get; set; }

        [XmlIgnore]
        public short CompanyId { get; set; }

        [XmlIgnore]
        public string CompanyName { get; set; }

        [XmlIgnore]
        public int UnitId { get; set; }

        [XmlIgnore]
        public string UnitName { get; set; }

        [XmlIgnore]
        public int DepartmentId { get; set; }

        [XmlIgnore]
        public string DepartmentName { get; set; }

        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string JobLevel { get; set; }

        [XmlIgnore]
        public int LocalCurrentHeadcount { get; set; }

        [XmlIgnore]
        public int ExpatCurrentHeadcount { get; set; }

        [XmlIgnore]
        public int TotalCurrentHeadcount => LocalCurrentHeadcount + ExpatCurrentHeadcount;

        [XmlIgnore]
        public int LocalApprovedHeadcount { get; set; }

        [XmlIgnore]
        public int ExpatApprovedHeadcount { get; set; }

        [XmlIgnore]
        public int TotalApprovedHeadcount => LocalApprovedHeadcount + ExpatApprovedHeadcount;

        public int LocalAddition { get; set; }
        public int ExpatAddition { get; set; }

        [XmlIgnore]
        public int TotalAddition => LocalAddition + ExpatAddition;
    }
}