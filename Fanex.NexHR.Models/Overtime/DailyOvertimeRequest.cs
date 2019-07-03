namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class DailyOvertimeRequest
    {
        public DateTime WorkDate { get; set; }
        public string Shift { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public decimal TotalWorkHours { get; set; }

        public decimal NormalHours { get; set; }
        public decimal OffHours { get; set; }
        public decimal PublicHours { get; set; }

        public string Remark { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsHoliday { get; set; }
    }
}