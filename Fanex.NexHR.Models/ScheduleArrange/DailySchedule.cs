namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Fanex.NexHR.Core.Constants;

    public class DailySchedule
    {
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
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
        public bool IsModified { get; set; }

        public LeaveRequest LeaveType { get; set; }
        public LeaveRequest LeaveType2 { get; set; }

        public bool IsPublic { get; set; }

        public bool IsLeaveHalfDay => IsLeaveDay && !IsHoliday && !IsOffDay && LeaveDay == Constants.HalfDayOffValue;
    }
}