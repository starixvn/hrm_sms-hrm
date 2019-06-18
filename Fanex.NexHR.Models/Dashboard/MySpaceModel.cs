namespace Fanex.NexHR.Models
{
    public class MySpaceModel
    {
        public decimal TotalPendingRequest { get; set; }

        public decimal TotalCheckInOutRequest { get; set; }

        public decimal TotalPendingCheckInOutRequest { get; set; }

        public decimal TotalLateInEarlyOutRequest { get; set; }

        public decimal TotalPendingLateInEarlyOutRequest { get; set; }

        public decimal TotalChangedShiftRequest { get; set; }

        public decimal TotalPendingChangedShiftRequest { get; set; }

        public decimal TotalLeaveRequest { get; set; }

        public decimal TotalPendingLeaveRequest { get; set; }

        public decimal TotalKivRequest { get; set; }

        public decimal TotalPendingKivRequest { get; set; }

        public decimal ForgotInOut { get; set; }

        public decimal LateInEarlyOut { get; set; }

        public decimal TotalPendingOverTimeRequest { get; set; }
    }
}