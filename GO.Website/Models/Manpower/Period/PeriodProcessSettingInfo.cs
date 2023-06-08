namespace GO.Website.Models.Manpower.Period
{
    using GO.Core.Extensions.XML;
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XMLName("Row")]
    public class PeriodProcessSettingInfo
    {
        [XmlIgnore]
        public int PeriodId { get; set; }

        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
