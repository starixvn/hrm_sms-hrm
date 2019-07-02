namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Overtime;

    public class EmployeeMonthlyOvertimeViewModel
    {
        public string Employee { get; set; }
        public int EmployeeId { get; set; }
        public int Year { get; set; } = DateTime.Now.Year;
        public IEnumerable<MonthlyOvertimeRequest> OvertimeRequests { get; set; }
    }
}