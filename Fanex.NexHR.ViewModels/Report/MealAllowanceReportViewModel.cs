namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class MealAllowanceReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte? IsLocal { get; set; }
        public string TeamIds { get; set; }
        public int? EmployeeId { get; set; }
        public int Employee { get; set; }
        public IEnumerable<EntityBase<int>> Teams { get; set; }
    }
}
