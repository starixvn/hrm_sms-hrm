namespace GO.WebAPI.Models.UnitHead
{
    using System;
    using System.Xml.Serialization;
    using GO.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class UnitHeadSchedule
    {
        [XmlIgnore]
        public short CompanyId { get; set; }

        [XmlIgnore]
        public short SiteId { get; set; }

        public int EmpId { get; set; }
        public DateTime WorkDate { get; set; }
        public bool IsWorkDay { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsLeaveDay { get; set; }

        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
    }
}