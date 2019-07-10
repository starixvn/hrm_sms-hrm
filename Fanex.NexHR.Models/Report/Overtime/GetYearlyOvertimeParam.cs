namespace Fanex.NexHR.Models.Report.Overtime
{
    public class GetYearlyOvertimeParam
    {
        public int Year { get; set; }
        public int? EmployeeId { get; set; }
        public string SectionIds { get; set; }
        public byte Calendar { get; set; }
    }
}