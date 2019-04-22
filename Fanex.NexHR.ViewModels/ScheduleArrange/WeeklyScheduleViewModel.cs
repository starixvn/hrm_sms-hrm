namespace Fanex.NexHR.ViewModels.ScheduleArrange
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.ScheduleArrange;

    public class WeeklyScheduleViewModel
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public IEnumerable<WeeklySchedule> Members { get; set; } = new List<WeeklySchedule>();
    }
}