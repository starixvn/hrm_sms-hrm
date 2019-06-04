namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Newtonsoft.Json;

    public class WeeklySchedule
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public EntityBase<int> Team { get; set; }
        public string Remark { get; set; }

        [JsonIgnore]
        public string PickupRemark { get; set; }
        public int DisplayOrder { get; set; }
        public ShiftBase DefaultShift { get; set; }
        public short OffDaysbyWeek { get; set; }
        public bool IsUsedWeeklyDefaultShift { get; set; }
        public WeeklyDefaultShift WeeklyDefaultShift { get; set; }

        public string Modifier { get; set; }
        public DateTime ModifiedDate { get; set; }

        public DailySchedule MondaySchedule { get; set; }
        public DailySchedule TuesdaySchedule { get; set; }
        public DailySchedule WednesdaySchedule { get; set; }
        public DailySchedule ThursdaySchedule { get; set; }
        public DailySchedule FridaySchedule { get; set; }
        public DailySchedule SaturdaySchedule { get; set; }
        public DailySchedule SundaySchedule { get; set; }
    }
}