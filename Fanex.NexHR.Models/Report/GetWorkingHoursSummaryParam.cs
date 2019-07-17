namespace Fanex.NexHR.Models.Report
{
    using System;

    public class GetWorkingHoursSummaryParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte? IsLocal { get; set; }
        public string SectionIds { get; set; }
        public int? EmployeeId { get; set; }
    }
}