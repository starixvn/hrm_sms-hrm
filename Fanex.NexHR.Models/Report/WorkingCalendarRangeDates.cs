namespace Fanex.NexHR.Models.Report
{
    using Fanex.NexHR.Models.ScheduleArrange;
    using System.Collections.Generic;

    public class WorkingCalendarRangeDates
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Team { get; set; }
        public string SectionCode { get; set; }
        public string DepartmentCode { get; set; }
        public IEnumerable<DailySchedule> schedules { get; set; }
    }
}