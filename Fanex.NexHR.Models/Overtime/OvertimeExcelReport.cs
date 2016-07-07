namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    public class OvertimeExcelReport
    {
        public string Team { get; set; }

        public string Positions { get; set; }

        public int EmpId { get; set; }

        public string EmpCode { get; set; }

        public string Fullname { get; set; }

        public string Nickname { get; set; }

        public double TotalWorkDays { get; set; }

        public double UnpaidLeave { get; set; }

        public double Normal { get; set; }

        public double Offday { get; set; }

        public double Holiday { get; set; }

        public double Tax { get; set; }

        public IList<OvertimeDailyReport> OvertimeDailyReport { get; set; }
    }
}