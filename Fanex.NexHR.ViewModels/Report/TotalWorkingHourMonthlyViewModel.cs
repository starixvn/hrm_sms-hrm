namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class TotalWorkingHourMonthlyViewModel
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }
    }
}