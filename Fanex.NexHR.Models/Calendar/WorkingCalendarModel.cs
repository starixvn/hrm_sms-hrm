namespace Fanex.NexHR.Models
{
    using System;

    public class WorkingCalendarModel
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public Shift Shift { get; set; }

        public bool IsHoliday { get; set; }

        public bool IsOffDay { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsLeaveOffDay { get; set; }

        public bool IsOvertime { get; set; }

        public decimal LeaveDay { get; set; }

        public bool IsKeepInViewDay { get; set; }

        public bool IsChangeShift { get; set; }

        public LeaveType LeaveType { get; set; }

        public LeaveType LeaveType2 { get; set; }

        public int Weekday { get; set; }

        public string ModifierName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsPublic { get; set; }

        public string ShiftMultipleID { get; set; }

        public string ShiftMultipleCode { get; set; }

        public short OffDaysbyWeek { get; set; }
    }
}