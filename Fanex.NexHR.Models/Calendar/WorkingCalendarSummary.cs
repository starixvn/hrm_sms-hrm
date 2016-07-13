namespace Fanex.NexHR.Models
{
    using System;

    public class WorkingCalendarSummary
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Schedule { get; set; }

        public DateTime WorkDate { get; set; }
    }
}