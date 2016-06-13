namespace Fanex.NexHR.Models
{
    public class RequestsMonitoring
    {
        public Employee Requester { get; set; }

        public int TotalCheckInOut { get; set; }

        public int TotalLateIn { get; set; }

        public int TotalEarlyOut { get; set; }

        public int TotalChangeShift { get; set; }

        public int TotalLeave { get; set; }

        public int TotalUnpaidLeave { get; set; }

        public int TotalOvertime { get; set; }

        public int KeepInViewRequest { get; set; }
    }
}
