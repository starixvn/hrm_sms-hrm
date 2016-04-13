namespace Fanex.NexHR.ViewModels.Discipline
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class DisciplineSummaryViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        [Display(Name = "Team")]
        public int? TeamId { get; set; }

        public bool? IsLocal { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        [Display(Name = "Reason")]
        public int? ReasonId { get; set; }

        public bool ShowAll { get; set; }
    }
}