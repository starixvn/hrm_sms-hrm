namespace Fanex.NexHR.Models
{
    using System;

    public class WeekWorking
    {
        public long Id { get; set; }

        public int EmpId { get; set; }

        public DateTime WorkDate { get; set; }

        public int? ShiftId { get; set; }

        public bool? IsHoliday { get; set; }

        public bool? IsOffDay { get; set; }

        public bool? IsLeaveDay { get; set; }

        public int? LeaveTypeId { get; set; }

        public int? LateIn { get; set; }

        public int? EarlyOut { get; set; }

        public DateTime? RequestedCheckinTime { get; set; }

        public DateTime? RequestedCheckoutTime { get; set; }

        public long? ChangeShiftId { get; set; }
    }
}
