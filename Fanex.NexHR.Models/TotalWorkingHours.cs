namespace Fanex.NexHR.Models
{
    public class TotalWorkingHours
    {
        public Employee Employee { get; set; }

        public int WorkingDay { get; set; }

        public float ActualWorkingDay { get; set; }

        public float LeaveTaken { get; set; }

        public float UnpaidLeave { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public float TotalHours { get; set; }

        public float AverageHours { get; set; }
    }
}
