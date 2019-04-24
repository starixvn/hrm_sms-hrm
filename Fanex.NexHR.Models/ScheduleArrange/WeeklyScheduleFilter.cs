namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;

    public class WeeklyScheduleFilter
    {
        public int ActorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupIds { get; set; }
        public int? EmployeeId { get; set; }
    }
}