namespace Fanex.NexHR.Models.SyncData
{
    using System;

    public class WorkingShiftSyncModel
    {
        public string EmployeeID { get; set; }

        public string ShiftCode { get; set; }

        public string ShiftDate { get; set; }

        public string ShiftStartTime { get; set; }

        public string ShiftEndTime { get; set; }

        public string LastUpdated { get; set; }

        public int IsInAfterDay { get; set; }

        public int IsOutAfterDay { get; set; }

        public int IsCrossDay { get; set; }

        public double TotalHours { get; set; }

        public string CheckInTime { get; set; } = "00:00";

        public string CheckOutTime { get; set; } = "00:00";
    }
}
