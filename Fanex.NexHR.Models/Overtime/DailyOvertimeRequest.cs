namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using System.Xml.Serialization;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class DailyOvertimeRequest
    {
        public DateTime WorkDate { get; set; }
        public int? ShiftId { get; set; }

        [XmlIgnore]
        public string ShiftCode { get; set; }

        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public decimal ActualWorkingHours { get; set; }

        public decimal NormalHours { get; set; }
        public decimal OffHours { get; set; }
        public decimal PublicHours { get; set; }

        public string Remark { get; set; }

        [XmlIgnore]
        public bool IsOffDay { get; set; }

        [XmlIgnore]
        public bool IsHoliday { get; set; }

        [XmlIgnore]
        public bool IsKeepInView { get; set; }
    }
}