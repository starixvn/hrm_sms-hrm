namespace Fanex.NexHR.Models.Report.MonthlyWorkingHours
{
    using System;

    public class GetMonthlyWorkingHoursParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SectionIds { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsLocal { get; set; }
    }
}