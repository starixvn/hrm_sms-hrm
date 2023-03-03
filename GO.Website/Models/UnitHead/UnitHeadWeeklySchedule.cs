namespace GO.Website.Models.UnitHead
{
    using System;

    [Serializable]
    public class UnitHeadWeeklySchedule : UnitHeadInfo
    {
        public UnitHeadDailySchedule MondaySchedule { get; set; }
        public UnitHeadDailySchedule TuesdaySchedule { get; set; }
        public UnitHeadDailySchedule WednesdaySchedule { get; set; }
        public UnitHeadDailySchedule ThursdaySchedule { get; set; }
        public UnitHeadDailySchedule FridaySchedule { get; set; }
        public UnitHeadDailySchedule SaturdaySchedule { get; set; }
        public UnitHeadDailySchedule SundaySchedule { get; set; }
    }
}