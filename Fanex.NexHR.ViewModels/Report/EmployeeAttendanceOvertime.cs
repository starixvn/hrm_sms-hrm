namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using Fanex.NexHR.Models.Overtime;

    public class EmployeeAttendanceOvertime
    {
        public string OvertimeName { get; set; }

        public OvertimeTypeModel OvertimeType { get; set; }

        public DateTime? OvertimeFromTime { get; set; }

        public DateTime? OvertimeToTime { get; set; }

        public decimal? OvertimeTotalHours { get; set; }
    }
}
