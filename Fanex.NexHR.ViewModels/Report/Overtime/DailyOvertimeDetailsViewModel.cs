namespace Fanex.NexHR.ViewModels.Report.Overtime
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.Overtime;

    public class DailyOvertimeDetailsViewModel
    {
        public DateTime FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public string SectionIds { get; set; }
        public IEnumerable<EntityBase<int>> Sections { get; set; }
        public string Employee { get; set; }
        public byte? IsLocal { get; set; }
        public byte? EmployeeStatusId { get; set; } = (byte)EmployeeStatus.Enabled;
        public IEnumerable<DailyOvertimeDetails> Overtimes { get; set; } = new List<DailyOvertimeDetails>();
    }
}