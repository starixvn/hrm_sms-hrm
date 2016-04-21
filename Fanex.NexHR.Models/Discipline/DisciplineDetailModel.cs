namespace Fanex.NexHR.Models.Discipline
{
    using System;

    public class DisciplineDetailModel
    {
        public DateTime WorkDate { get; set; }

        public string ShiftName { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public EmployeeInfoModel Employee { get; set; }

        public ViolationModel MissingCheckInOut { get; set; }

        public ViolationModel TotalHours { get; set; }

        public ViolationModel LateIn { get; set; }

        public ViolationModel EarlyOut { get; set; }

        public ViolationModel Absent { get; set; }

        public ViolationModel UnpaidLeaveAbsent { get; set; }

        public ViolationModel UnpaidLeave { get; set; }

        public ViolationModel ChangeShift { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public decimal ApproverSubtractScore { get; set; }

        public string ApproverComment { get; set; }

        public string EmployeeComment { get; set; }
    }
}