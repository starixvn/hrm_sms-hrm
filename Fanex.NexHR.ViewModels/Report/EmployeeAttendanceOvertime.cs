namespace Fanex.NexHR.ViewModels
{
    using System;
    using Fanex.NexHR.Models;

    public class EmployeeAttendanceOvertime
    {
        public string OvertimeName { get; set; }

        public OvertimeTypeModel OvertimeType { get; set; }

        public DateTime? OvertimeFromTime { get; set; }

        public DateTime? OvertimeToTime { get; set; }

        public decimal? OvertimeTotalHours { get; set; }
    }
}