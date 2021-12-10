namespace Fanex.NexHR.Models.SyncData
{
    using System;
    using System.Xml.Serialization;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class WorkingShiftSyncModel
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public string EmployeeID { get; set; }

        public string ShiftCode { get; set; }

        public DateTime ShiftDate { get; set; }

        [XmlIgnore]
        public DateTime? ShiftStartTime { get; set; }

        [XMLName("ShiftStartTime")]
        public string ShiftStartTimeXml => FormatDateTime(ShiftStartTime);

        [XmlIgnore]
        public DateTime? ShiftEndTime { get; set; }

        [XMLName("ShiftEndTime")]
        public string ShiftEndTimeXml => FormatDateTime(ShiftEndTime);

        public DateTime LastUpdated { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public bool IsCrossDay { get; set; }

        public double TotalHours { get; set; }

        public string CheckInTime { get; set; } = "00:00";

        public string CheckOutTime { get; set; } = "00:00";

        private static string FormatDateTime(DateTime? date)
            => date.HasValue ? date.Value.ToString(DateTimeFormat) : string.Empty;
    }
}
