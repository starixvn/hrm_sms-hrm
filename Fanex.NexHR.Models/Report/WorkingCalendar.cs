namespace Fanex.NexHR.Models.Report
{
    using Fanex.NexHR.Models.ScheduleArrange;

    public class WorkingCalendar
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Team { get; set; }
        public string SectionCode { get; set; }
        public string DepartmentCode { get; set; }
        public DailySchedule MondaySchedule { get; set; }
        public DailySchedule TuesdaySchedule { get; set; }
        public DailySchedule WednesdaySchedule { get; set; }
        public DailySchedule ThursdaySchedule { get; set; }
        public DailySchedule FridaySchedule { get; set; }
        public DailySchedule SaturdaySchedule { get; set; }
        public DailySchedule SundaySchedule { get; set; }
    }
}