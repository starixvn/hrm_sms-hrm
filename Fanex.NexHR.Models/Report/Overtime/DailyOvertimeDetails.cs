namespace Fanex.NexHR.Models.Report.Overtime
{
    using Fanex.NexHR.Models.Overtime;

    public class DailyOvertimeDetails : DailyOvertimeRequest
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
    }
}