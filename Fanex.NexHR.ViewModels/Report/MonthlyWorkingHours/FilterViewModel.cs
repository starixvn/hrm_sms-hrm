namespace Fanex.NexHR.ViewModels.Report.MonthlyWorkingHours
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class FilterViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string SectionIds { get; set; }
        public IEnumerable<EntityBase<int>> Sections { get; set; }
        public ushort Calendar { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsLocal { get; set; }
    }
}