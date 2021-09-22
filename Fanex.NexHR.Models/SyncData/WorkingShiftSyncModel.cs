namespace Fanex.NexHR.Models.SyncData
{
    using System;

    public class WorkingShiftSyncModel
    {
        public string EmployeeID { get; set; }

        public string ShiftCode { get; set; }

        public DateTime ShiftDate { get; set; }

        public DateTime ShiftStartTime { get; set; }

        public DateTime ShiftEndTime { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
