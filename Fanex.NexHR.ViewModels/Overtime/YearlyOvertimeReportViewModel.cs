namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class YearlyOvertimeReportViewModel
    {
        public int Year { get; set; }

        public int? EmployeeId { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public int Calendar { get; set; }
    }
}