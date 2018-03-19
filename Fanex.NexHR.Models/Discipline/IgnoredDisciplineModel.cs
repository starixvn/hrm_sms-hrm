namespace Fanex.NexHR.Models.Discipline
{
    using System;

    public class IgnoredDisciplineModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public int DisciplineTypeId { get; set; }

        public bool IsIgnored { get; set; }

        public bool IsIgnoredChangeShift { get; set; }

        public bool IsIgnoredMissingCheckInOut { get; set; }

        public bool IsIgnoredLateIn { get; set; }

        public bool IsIgnoredEarlyOut { get; set; }

        public bool IsIgnoredAbsent { get; set; }

        public bool IsIgnoredUnplannedLeave { get; set; }

        public bool IsIgnoredUnpaidLeave { get; set; }

        public bool IsIgnoredWorkingUnderTime { get; set; }

        public bool IsIgnoredUnpaidLeaveAbsent { get; set; }

        public bool IsIgnoredWeekendLeave { get; set; }

        public bool IsConfirmed
        {
            get
            {
                return !IsIgnored;
            }
        }

        public decimal ApproverSubtractScore { get; set; }

        public bool IsOutOfControlReason { get; set; }
    }
}