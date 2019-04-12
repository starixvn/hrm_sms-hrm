namespace Fanex.NexHR.Models.ScheduleArrange
{
    public class WeeklySchedule
    {
        public int EmployeeId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public EntityBase<int> Team { get; set; }
        public string Remark { get; set; }
        public EntityBase<int> DefaultShift { get; set; }
        public short OffDaysbyWeek { get; set; }

        public DailySchedule MondaySchedule { get; set; }
        public DailySchedule TuesdaySchedule { get; set; }
        public DailySchedule WednesdaySchedule { get; set; }
        public DailySchedule ThursdaySchedule { get; set; }
        public DailySchedule FridaySchedule { get; set; }
        public DailySchedule SaturdaySchedule { get; set; }
        public DailySchedule SundaySchedule { get; set; }
    }
}