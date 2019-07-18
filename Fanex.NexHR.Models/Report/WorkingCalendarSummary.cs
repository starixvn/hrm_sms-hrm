namespace Fanex.NexHR.Models.Report
{
    using System;

    public class WorkingCalendarSummary
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }
        public DateTime WorkDate { get; set; }
        public string Shift { get; set; }
    }
}