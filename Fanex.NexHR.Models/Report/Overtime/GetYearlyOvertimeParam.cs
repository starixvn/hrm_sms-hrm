namespace Fanex.NexHR.Models.Report.Overtime
{
    public class GetYearlyOvertimeParam
    {
        public int ActorId { get; set; }
        public int Year { get; set; }
        public string Employee { get; set; }
        public string SectionIds { get; set; }
        public byte Calendar { get; set; }
        public byte? EmployeeStatusId { get; set; }
    }
}