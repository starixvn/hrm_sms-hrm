namespace Fanex.NexHR.Models.Calendar
{
    using System;

    public class ScheduleModel
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public int? ShiftId { get; set; }

        public bool IsHoliday { get; set; }

        public bool IsOffDay { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsKeepInView { get; set; }

        public int LeaveTypeId { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public DateTime RequestCheckInTime { get; set; }

        public DateTime RequestCheckOutTime { get; set; }

        public int ChangeShiftId { get; set; }

        public decimal LeaveDay { get; set; }

        public bool IsPublic { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}
