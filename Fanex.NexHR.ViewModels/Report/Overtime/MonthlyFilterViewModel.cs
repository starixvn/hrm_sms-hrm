namespace Fanex.NexHR.ViewModels.Report.Overtime
{
    using System;

    public class MonthlyFilterViewModel
    {
        private static DateTime Today = DateTime.Today;
        public DateTime FromDate { get; set; } = Today;
        public DateTime ToDate { get; set; } = Today;
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; }
    }
}