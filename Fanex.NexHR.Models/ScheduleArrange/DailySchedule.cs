namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;

    public class DailySchedule
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }
        public int Weekday { get; set; }

        public ShiftBase Shift { get; set; }

        public bool IsHoliday { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsLeaveDay { get; set; }
        public bool IsLeaveOffDay { get; set; }
        public bool IsOvertime { get; set; }
        public decimal LeaveDay { get; set; }
        public bool IsKeepInView { get; set; }
        public bool IsChangeShift { get; set; }
        public bool IsOffDayReplacement { get; set; }

        public LeaveRequest LeaveType { get; set; }
        public LeaveRequest LeaveType2 { get; set; }

        public bool IsPublic { get; set; }

        public bool IsLeaveHalfDay => IsLeaveDay && !IsHoliday && !IsOffDay && LeaveDay == 0.5M;
    }
}