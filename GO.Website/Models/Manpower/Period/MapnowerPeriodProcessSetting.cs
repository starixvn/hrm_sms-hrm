namespace GO.Website.Models.Manpower.Period
{
    using System;
    using System.Xml.Serialization;
    using GO.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class MapnowerPeriodProcessSetting
    {
        [XmlIgnore]
        public short PeriodId { get; set; }

        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}