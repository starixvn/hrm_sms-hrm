namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;

    public class DailySchedule
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }
        public int Weekday { get; set; }

        public EntityBase<int> Shift { get; set; }

        public bool IsHoliday { get; set; }

        public bool IsOffDay { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsLeaveOffDay { get; set; }

        public bool IsOvertime { get; set; }

        public decimal LeaveDay { get; set; }

        public bool IsKeepInViewDay { get; set; }

        public bool IsChangeShift { get; set; }

        public EntityBase<int> LeaveType { get; set; }
        public EntityBase<int> LeaveType2 { get; set; }

        public bool IsPublic { get; set; }
    }
}