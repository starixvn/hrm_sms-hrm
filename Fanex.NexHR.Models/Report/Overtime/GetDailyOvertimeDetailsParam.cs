namespace Fanex.NexHR.Models.Report.Overtime
{
    using System;

    public class GetDailyOvertimeDetailsParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Employee { get; set; }
        public string SectionIds { get; set; }
        public byte? IsLocal { get; set; }
        public byte? EmployeeStatusId { get; set; }
    }
}