namespace Fanex.NexHR.Models.Report
{
    using Fanex.NexHR.Models.ScheduleArrange;

    public class WorkingCalendarSummary : DailySchedule
    {
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }
    }
}