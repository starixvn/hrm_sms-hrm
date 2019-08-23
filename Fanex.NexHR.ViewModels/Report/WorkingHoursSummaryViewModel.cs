namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;

    public class WorkingHoursSummaryViewModel
    {
        private static DateTime Today = DateTime.Today;

        public byte DateOption { get; set; } = (byte)DateRangeOption.CurrentMonth;
        public DateTime FromDate { get; set; } = new DateTime(Today.Year, Today.Month, 1);
        public DateTime ToDate { get; set; } = Today;
        public byte? IsLocal { get; set; }
        public string SectionIds { get; set; }
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; } = (byte)EmployeeStatus.Enabled;

        public IEnumerable<EntityBase<int>> Sections { get; set; }
        public IEnumerable<WorkingHoursSummary> WorkingHours { get; set; } = new List<WorkingHoursSummary>();
    }
}