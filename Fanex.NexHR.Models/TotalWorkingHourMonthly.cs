namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    public class TotalWorkingHourMonthly
    {
        public string StaffId { get; set; }

        public string Section { get; set; }

        public string FullNameNick { get; set; }

        public string Nick { get; set; }

        public decimal CountWorkDay { get; set; }

        public List<string> WorkingHours { get; set; }
    }
}