namespace Fanex.NexHR.Models.Report
{
    public class WeekendLeaveSummary
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public bool IsLocal { get; set; }

        public int GroupId { get; set; }

        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public double TotalWeekendLeave { get; set; }
    }
}