namespace Fanex.NexHR.ViewModels.Report.WorkingHours
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class MonthlyFilterViewModel
    {
        public string SectionIds { get; set; }
        public IEnumerable<EntityBase<int>> Sections { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsLocal { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}