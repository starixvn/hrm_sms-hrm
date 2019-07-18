namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class WorkingCalendarSummaryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte? IsLocal { get; set; }
        public string SectionIds { get; set; }
        public string Employee { get; set; }
        public int? EmployeeId { get; set; }

        public IEnumerable<EntityBase<int>> Sections { get; set; }
    }
}